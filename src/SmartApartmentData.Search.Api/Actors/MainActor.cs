using Akka.Actor;
using Microsoft.Extensions.Options;
using SmartDataApartment.Search.Api.Actors.Messages;
using SmartDataApartment.Search.Api.Configurations;

namespace SmartDataApartment.Search.Api.Actors;

public class MainActor : ReceiveActor
{
    private readonly ILogger<MainActor> _logger;
    private readonly BulkUploadOptions _bulkUploadOptions;

    public MainActor(ILogger<MainActor> logger,
        IOptions<BulkUploadOptions> bulkUploadOptions)
    {
        _logger = logger;
        _bulkUploadOptions = bulkUploadOptions.Value;

        ReceiveAsync<BulkManagementCompanyUploadMessage>(ProcessManagementCompanies);
        ReceiveAsync<BulkPropertiesUploadMessage>(ProcessProperties);
    }

    private async Task ProcessManagementCompanies(BulkManagementCompanyUploadMessage message)
    {
        try
        {
            await Task.Delay(0);
            var distinctManagementCompanyList = message.ManagementCompanies
                .DistinctBy(m => m.ManagementCompany.ManagementId)
                .Select(x => x.ManagementCompany)
                .Chunk(_bulkUploadOptions.ChunkSize)
                .ToList();
            
            distinctManagementCompanyList.ForEach(chunk => 
                TopLevelActors.PersistManagementCompaniesToEsActor.Tell(new ManagementListMessage(chunk.ToList()), ActorRefs.Nobody));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured processing management companies to be stored in es");
        }
    }
    
    private async Task ProcessProperties(BulkPropertiesUploadMessage message)
    {
        try
        {
            await Task.Delay(0);
            var distinctPropertiesList = message.Properties
                .DistinctBy(m => m.Property.PropertyId)
                .Select(x => x.Property)
                .Chunk(_bulkUploadOptions.ChunkSize)
                .ToList();
            
            distinctPropertiesList.ForEach(chunk => 
                TopLevelActors.PersistPropertiesToEsActor.Tell(new PropertyListMessage(chunk), ActorRefs.Nobody));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occured processing properties to be stored in es");
        }
    }
}