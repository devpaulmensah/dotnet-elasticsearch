using System.ComponentModel.DataAnnotations;

namespace SmartDataApartment.Search.Api.Attributes;

public class AllowedFileExtensionAttribute : ValidationAttribute
{
    private readonly string[] _extensions;

    public AllowedFileExtensionAttribute(string[] extensions)
    {
        _extensions = extensions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var file = value as IFormFile;

        if (file is not null)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            if (!_extensions.Contains(fileExtension.ToLower()))
                return new ValidationResult("Only json files are allowed");
        }

        return ValidationResult.Success;
    }
}