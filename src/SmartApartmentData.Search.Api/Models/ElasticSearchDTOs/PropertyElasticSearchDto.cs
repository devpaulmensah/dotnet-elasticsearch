using Nest;
using Newtonsoft.Json;

namespace SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;

[ElasticsearchType(IdProperty = "propertyID", RelationName = nameof(PropertyElasticSearchDto))]
public class PropertyElasticSearchDto : PropertyBase
{
    [JsonProperty("propertyID")] 
    public int PropertyId { get; set; }
    public string FormerName { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    [JsonProperty("lat")] 
    public float Latitude { get; set; }
    [JsonProperty("lng")] 
    public float Longitude { get; set; }
}

public class PropertyUploadRequest
{
    public PropertyElasticSearchDto Property { get; set; }
}