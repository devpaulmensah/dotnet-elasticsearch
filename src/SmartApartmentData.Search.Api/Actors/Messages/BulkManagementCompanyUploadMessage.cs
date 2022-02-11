using SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;

namespace SmartDataApartment.Search.Api.Actors.Messages;

public struct BulkManagementCompanyUploadMessage
{
    public IEnumerable<ManagementCompanyUploadRequest> ManagementCompanies { get;}

    public BulkManagementCompanyUploadMessage(IEnumerable<ManagementCompanyUploadRequest> managementCompanies)
    {
        ManagementCompanies = managementCompanies;
    }
}