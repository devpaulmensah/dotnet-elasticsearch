using Akka.Actor;
using Nest;
using SmartDataApartment.Search.Api.Actors.Messages;
using SmartDataApartment.Search.Api.Helpers;

namespace SmartDataApartment.Search.Api.Actors.ElasticSearchActors;

public class PersistManagementCompaniesToEsActor : ReceiveActor
{
    private readonly ILogger<PersistManagementCompaniesToEsActor> _logger;
    private readonly IElasticClient _elasticClient;

    public PersistManagementCompaniesToEsActor(ILogger<PersistManagementCompaniesToEsActor> logger,
        IElasticClient elasticClient)
    {
        _logger = logger;
        _elasticClient = elasticClient;

        ReceiveAsync<ManagementListMessage>(PersistRecordsToEs);
    }

    private async Task PersistRecordsToEs(ManagementListMessage message)
    {
        try
        {
            var bulkUploadResponse = await _elasticClient.BulkAsync(c => c
                .Index(CommonConstants.ManagementsCompaniesIndex)
                .IndexMany(message.ManagementList, (desc, document) =>
                    desc.Id(document.ManagementId)));
            
            if (!bulkUploadResponse.IsValid)
            {
                _logger.LogError("An error occured uploading management companies" +
                                 $"\nDebugInformation: {bulkUploadResponse?.DebugInformation}");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured persisting management companies to elastic search");
        }
    }
}