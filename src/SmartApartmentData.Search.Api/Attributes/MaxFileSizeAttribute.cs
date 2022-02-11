using System.ComponentModel.DataAnnotations;

namespace SmartDataApartment.Search.Api.Attributes;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;

    public MaxFileSizeAttribute(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        var file = value as IFormFile;

        if (file != null)
        {
            var fileSize = file.Length;
            if (fileSize > _maxFileSize) return new ValidationResult("Maximum file size allowed is 20MB");
        }

        return ValidationResult.Success;
    }
}