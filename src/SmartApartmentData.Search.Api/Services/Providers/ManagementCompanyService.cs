using Akka.Actor;
using Newtonsoft.Json;
using SmartDataApartment.Search.Api.Actors;
using SmartDataApartment.Search.Api.Actors.Messages;
using SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;
using SmartDataApartment.Search.Api.Models.Responses;
using SmartDataApartment.Search.Api.Services.Interfaces;

namespace SmartDataApartment.Search.Api.Services.Providers;

public class ManagementCompanyService : IManagementCompanyService
{
    private readonly ILogger<ManagementCompanyService> _logger;

    public ManagementCompanyService(ILogger<ManagementCompanyService> logger)
    {
        _logger = logger;
    }

    public async Task<BaseResponse<NoDataResponse>> ProcessFile(IFormFile file)
    {
        try
        {
            try
            {
                string content;

                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    content = await reader.ReadToEndAsync();
                }
                
                var managementCompanyList =
                    JsonConvert.DeserializeObject<List<ManagementCompanyUploadRequest>>(content);

                if (managementCompanyList is null || !managementCompanyList.Any())
                {
                    return CommonResponses.ErrorResponse
                        .BadRequestResponse<NoDataResponse>("File empty");
                }
                
                if (managementCompanyList.Any(m => m.ManagementCompany?.ManagementId is null))
                {
                    return CommonResponses.ErrorResponse
                        .BadRequestResponse<NoDataResponse>("Provide data that matches specification for management companies");
                }

                var response = await AddBulk(managementCompanyList);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured parsing management companies data");
                
                return CommonResponses.ErrorResponse
                    .BadRequestResponse<NoDataResponse>("Incorrect data");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured process management companies files");
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<NoDataResponse>();
        }
    }

    private async Task<BaseResponse<NoDataResponse>> AddBulk(IEnumerable<ManagementCompanyUploadRequest> managementCompanyList)
    {
        try
        {
            await Task.Delay(0);
            var eventMessage = new BulkManagementCompanyUploadMessage(managementCompanyList);

            TopLevelActors.MainActor.Tell(eventMessage, ActorRefs.Nobody);

            return CommonResponses.SuccessResponse.OkResponse(new NoDataResponse(),
                "Management companies list will be processed and uploaded shortly");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured uploading bulk managements");
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<NoDataResponse>();
        }
    }
}