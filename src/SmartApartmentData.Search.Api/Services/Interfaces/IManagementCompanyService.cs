using SmartDataApartment.Search.Api.Models.Responses;

namespace SmartDataApartment.Search.Api.Services.Interfaces;

public interface IManagementCompanyService
{
    Task<BaseResponse<NoDataResponse>> ProcessFile(IFormFile file);
}