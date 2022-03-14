using Nest;
using Newtonsoft.Json;
using SmartDataApartment.Search.Api.Helpers;
using SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;
using SmartDataApartment.Search.Api.Models.Requests;
using SmartDataApartment.Search.Api.Models.Responses;
using SmartDataApartment.Search.Api.Services.Interfaces;

namespace SmartDataApartment.Search.Api.Services.Providers;

public class ElasticSearchService : IElasticSearchService
{
    private readonly IElasticClient _elasticClient;
    private readonly ILogger<ElasticSearchService> _logger;

    public ElasticSearchService(IElasticClient elasticClient,
        ILogger<ElasticSearchService> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    public async Task<BaseResponse<SearchBaseResponse<dynamic>>> Search(SmartSearchRequest request)
    {
        try
        {
            request.Limit = request.Limit > 10000 ? 25 : request.Limit; 
            
            var propertiesSearchQuery = new QueryContainerDescriptor<PropertyElasticSearchDto>()
                .MultiMatch(m => m
                    .Fields(f => f
                        .Field(p => p.Name)
                        .Field(p => p.FormerName)
                        .Field(p => p.StreetAddress))
                    .Query(request.Query.ToLower()));

            if (request.Markets.Any())
            {
                propertiesSearchQuery = propertiesSearchQuery && new QueryContainerDescriptor<PropertyElasticSearchDto>()
                    .Terms(t => new TermsQuery
                    {
                        Field = "market",
                        Terms = request.Markets.Select(x => x.ToLower()).ToList()
                    });
            }

            var searchResponse = await _elasticClient.SearchAsync<dynamic>(descriptor => descriptor
                .Size(request.Limit)
                .Index(new [] {CommonConstants.ManagementsCompaniesIndex, CommonConstants.PropertiesIndex})
                .Query(q => propertiesSearchQuery));

            if (!searchResponse.IsValid)
            {
                _logger.LogDebug("An error occured while searching" +
                                 $"\nDebugInformation: {searchResponse.DebugInformation}");
                
                return CommonResponses.ErrorResponse
                    .FailedDependencyErrorResponse<SearchBaseResponse<dynamic>>();
            }

            var parsedResponse = searchResponse.Documents.Select(x => ParseResponse(x)).ToList();
            
            var smartSearchResponse = new SearchBaseResponse<dynamic>
            {
                Limit = request.Limit,
                TotalResults = int.Parse(searchResponse.HitsMetadata.Total.Value.ToString()),
                Results = parsedResponse
            };

            return CommonResponses.SuccessResponse.OkResponse(smartSearchResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured searching for properties\nSearchRequest:{request}", 
                JsonConvert.SerializeObject(request, Formatting.Indented));

            return CommonResponses.ErrorResponse.InternalServerErrorResponse<SearchBaseResponse<dynamic>>();
        }
    }

    private dynamic ParseResponse(dynamic document)
    {
        var data = (Dictionary<string, object>) document;

        data.TryGetValue("propertyId", out object propertyId);
        data.TryGetValue("managementId", out object managementId);

        if (!string.IsNullOrEmpty(Convert.ToString(propertyId)))
        {
            data.Add("type", CommonConstants.PropertyResponseType);
        }
        else if (!string.IsNullOrEmpty(Convert.ToString(managementId)))
        {
            data.Add("type", CommonConstants.ManagementCompanyResponseType);
        }
        
        return data;
    }
}