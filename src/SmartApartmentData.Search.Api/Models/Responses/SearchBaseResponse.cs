namespace SmartDataApartment.Search.Api.Models.Responses;

public class SearchBaseResponse<T>
{
    public int Limit { get; set; }
    public int TotalResults { get; set; }
    public List<T> Results { get; set; }
}