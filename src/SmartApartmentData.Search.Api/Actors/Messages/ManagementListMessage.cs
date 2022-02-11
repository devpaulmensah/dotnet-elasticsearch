using SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;

namespace SmartDataApartment.Search.Api.Actors.Messages;

public struct ManagementListMessage
{
    public List<ManagementCompanyElasticSearchDto> ManagementList { get; }

    public ManagementListMessage(List<ManagementCompanyElasticSearchDto> managementList)
    {
        ManagementList = managementList;
    }
}