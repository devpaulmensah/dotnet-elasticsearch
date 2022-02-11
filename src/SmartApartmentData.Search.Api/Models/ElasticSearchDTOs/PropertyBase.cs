using Nest;
using SmartDataApartment.Search.Api.Helpers;

namespace SmartDataApartment.Search.Api.Models.ElasticSearchDTOs;

public class PropertyBase
{
    [Text(Analyzer = CommonConstants.AutoCompleteAnalyzer, Name = "name")]
    public string Name { get; set; }

    [Text(Analyzer = CommonConstants.KeywordAnalyzer, Name = "market")]
    public string Market { get; set; }

    [Text(Analyzer = CommonConstants.KeywordAnalyzer, Name = "state")]
    public string State { get; set; }
}