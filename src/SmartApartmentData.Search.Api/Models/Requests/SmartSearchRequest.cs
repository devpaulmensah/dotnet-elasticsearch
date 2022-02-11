using System.ComponentModel.DataAnnotations;

namespace SmartDataApartment.Search.Api.Models.Requests;

public class SmartSearchRequest
{
    [Required(AllowEmptyStrings = false)]
    public string Query { get; set; }
    public List<string> Markets { get; set; } = new List<string>();
    public int Limit { get; set; } = 25;
}