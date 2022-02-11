using SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;

namespace SmartDataApartment.Search.Api.Actors.Messages;

public struct PropertyListMessage
{
    public IEnumerable<PropertyElasticSearchDto> PropertyList { get; }

    public PropertyListMessage(IEnumerable<PropertyElasticSearchDto> propertyList)
    {
        PropertyList = propertyList;
    }
}