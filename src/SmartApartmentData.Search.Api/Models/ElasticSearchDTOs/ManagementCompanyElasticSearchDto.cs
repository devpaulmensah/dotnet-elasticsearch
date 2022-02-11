using Nest;
using Newtonsoft.Json;

namespace SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;

[ElasticsearchType(IdProperty = "mgmtID", RelationName = nameof(ManagementCompanyElasticSearchDto))]
public class ManagementCompanyElasticSearchDto : PropertyBase
{
    [JsonProperty("mgmtID")] 
    public int ManagementId { get; set; }
}

public class ManagementCompanyUploadRequest
{
    [JsonProperty("mgmt")]
    public ManagementCompanyElasticSearchDto ManagementCompany { get; set; }
}