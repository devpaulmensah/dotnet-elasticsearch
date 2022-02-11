using SmartDataApartment.Search.Api.Models.Requests;
using SmartDataApartment.Search.Api.Models.Responses;

namespace SmartDataApartment.Search.Api.Services.Interfaces;

public interface IElasticSearchService
{
    Task<BaseResponse<SearchBaseResponse<dynamic>>> Search(SmartSearchRequest request);
}