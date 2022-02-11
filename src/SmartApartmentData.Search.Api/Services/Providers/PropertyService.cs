using Akka.Actor;
using Newtonsoft.Json;
using SmartDataApartment.Search.Api.Actors;
using SmartDataApartment.Search.Api.Actors.Messages;
using SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;
using SmartDataApartment.Search.Api.Models.Responses;
using SmartDataApartment.Search.Api.Services.Interfaces;

namespace SmartDataApartment.Search.Api.Services.Providers;

public class PropertyService : IPropertyService
{
    private readonly ILogger<PropertyService> _logger;

    public PropertyService(ILogger<PropertyService> logger)
    {
        _logger = logger;
    }

    public async Task<BaseResponse<NoDataResponse>> ProcessFile(IFormFile file)
    {
        try
        {
            string content;

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                content = await reader.ReadToEndAsync();
            }

            try
            {
                var propertiesList = JsonConvert.DeserializeObject<List<PropertyUploadRequest>>(content);

                if (propertiesList == null || !propertiesList.Any())
                {
                    return CommonResponses.ErrorResponse
                        .BadRequestResponse<NoDataResponse>("File empty");
                }
                
                if (propertiesList.Any(p => p.Property?.PropertyId == null))
                {
                    return CommonResponses.ErrorResponse
                        .BadRequestResponse<NoDataResponse>("Provide data that matches specification for properties");
                }

                var response = await AddBulk(propertiesList);

                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured parsing properties data");
                
                return CommonResponses.ErrorResponse
                    .BadRequestResponse<NoDataResponse>("Incorrect data");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured process properties files");
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<NoDataResponse>();
        }
    }

    private async Task<BaseResponse<NoDataResponse>> AddBulk(IEnumerable<PropertyUploadRequest> propertiesList)
    {
        try
        {
            await Task.Delay(0);
            var eventMessage = new BulkPropertiesUploadMessage(propertiesList);

            TopLevelActors.MainActor.Tell(eventMessage, ActorRefs.Nobody);

            return CommonResponses.SuccessResponse.OkResponse(new NoDataResponse(),
                "Properties list will be processed and uploaded shortly");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured uploading bulk properties");
            return CommonResponses.ErrorResponse.InternalServerErrorResponse<NoDataResponse>();
        }
    }
    
    
}