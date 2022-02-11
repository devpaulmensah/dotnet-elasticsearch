using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartDataApartment.Search.Api.Models.Requests;
using SmartDataApartment.Search.Api.Models.Responses;
using SmartDataApartment.Search.Api.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartDataApartment.Search.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponse<NoDataResponse>))]
public class SearchController : ControllerBase
{
    private readonly IElasticSearchService _elasticSearchService;

    public SearchController(IElasticSearchService elasticSearchService)
    {
        _elasticSearchService = elasticSearchService;
    }

    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<SearchBaseResponse<dynamic>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<NoDataResponse>))]
    [SwaggerOperation("Search", OperationId = "Search")]
    public async Task<IActionResult> Search([FromQuery] SmartSearchRequest request)
    {
        var response = await _elasticSearchService.Search(request);
        return StatusCode(response.Code, response);
    }
}