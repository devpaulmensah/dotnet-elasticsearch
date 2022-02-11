using System.ComponentModel.DataAnnotations;
using SmartDataApartment.Search.Api.Attributes;

namespace SmartDataApartment.Search.Api.Models.Requests;

public class FileUploadRequest
{
    [Required(ErrorMessage = "File is required")]
    [DataType(DataType.Upload)]
    [MaxFileSize(10000000)]
    [AllowedFileExtension(new [] { ".json"})]
    public IFormFile File { get; set; }
}
