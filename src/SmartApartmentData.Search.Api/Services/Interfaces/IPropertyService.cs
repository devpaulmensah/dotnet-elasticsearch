using SmartDataApartment.Search.Api.Models.Responses;

namespace SmartDataApartment.Search.Api.Services.Interfaces;

public interface IPropertyService
{
    Task<BaseResponse<NoDataResponse>> ProcessFile(IFormFile file);
}