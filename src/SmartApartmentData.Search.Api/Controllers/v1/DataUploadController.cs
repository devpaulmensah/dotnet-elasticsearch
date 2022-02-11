using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using SmartDataApartment.Search.Api.Models;
using SmartDataApartment.Search.Api.Models.Requests;
using SmartDataApartment.Search.Api.Models.Responses;
using SmartDataApartment.Search.Api.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace SmartDataApartment.Search.Api.Controllers.v1;

[ApiController]
[Route("api/v1/upload")]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(BaseResponse<NoDataResponse>))]
public class DataUploadController : ControllerBase
{
    private readonly IManagementCompanyService _managementCompanyService;
    private readonly IPropertyService _propertyService;

    public DataUploadController(IManagementCompanyService managementCompanyService,
        IPropertyService propertyService)
    {
        _managementCompanyService = managementCompanyService;
        _propertyService = propertyService;
    }

    [HttpPost("management-companies")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<NoDataResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<NoDataResponse>))]
    [SwaggerOperation("Upload json file containing management companies", OperationId = "UploadManagementCompanyFiles")]
    public async Task<IActionResult> UploadManagementCompanyFile([FromForm]  FileUploadRequest request)
    {
        var response = await _managementCompanyService.ProcessFile(request.File);

        return StatusCode(response.Code, response);
    }
    
    [HttpPost("properties")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BaseResponse<NoDataResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(BaseResponse<NoDataResponse>))]
    [SwaggerOperation("Upload json file containing properties", OperationId = "UploadPropertiesFile")]
    public async Task<IActionResult> UploadPropertiesFile([FromForm]  FileUploadRequest file)
    {
        var response = await _propertyService.ProcessFile(file.File);

        return StatusCode(response.Code, response);
    }
}
