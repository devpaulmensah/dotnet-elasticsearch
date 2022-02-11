using Akka.Actor;
using Nest;
using SmartDataApartment.Search.Api.Actors.Messages;
using SmartDataApartment.Search.Api.Helpers;

namespace SmartDataApartment.Search.Api.Actors.ElasticSearchActors;

public class PersistPropertiesToEsActor : ReceiveActor
{
    private readonly ILogger<PersistManagementCompaniesToEsActor> _logger;
    private readonly IElasticClient _elasticClient;

    public PersistPropertiesToEsActor(ILogger<PersistManagementCompaniesToEsActor> logger,
        IElasticClient elasticClient)
    {
        _logger = logger;
        _elasticClient = elasticClient;

        ReceiveAsync<PropertyListMessage>(PersistRecordsToEs);
    }
    
    private async Task PersistRecordsToEs(PropertyListMessage message)
    {
        try
        {
            var bulkUploadResponse = await _elasticClient.BulkAsync(c => c
                .Index(CommonConstants.PropertiesIndex)
                .IndexMany(message.PropertyList, (desc, document) =>
                    desc.Id(document.PropertyId)));
            
            if (!bulkUploadResponse.IsValid)
            {
                _logger.LogError("An error occured uploading properties" +
                                 $"\nDebugInformation: {bulkUploadResponse?.DebugInformation}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured persisting properties to elastic search");
        }
    }
}