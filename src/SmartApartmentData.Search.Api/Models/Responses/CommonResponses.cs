using System.Net;

namespace SmartDataApartment.Search.Api.Models.Responses;

public static class CommonResponses
{
    private const string InternalServerErrorResponseMessage = "Something bad happened, try again later";
    private const string FailedDependencyErrorResponseMessage = "An error occured, try again later";
    private const string DefaultOkResponseMessage = "Retrieved successfully";

    public static class ErrorResponse
    {
        public static BaseResponse<T> InternalServerErrorResponse<T>() =>
            new BaseResponse<T>
            {
                Code = (int) HttpStatusCode.InternalServerError,
                Message = InternalServerErrorResponseMessage
            };

        public static BaseResponse<T> FailedDependencyErrorResponse<T>() =>
            new BaseResponse<T>
            {
                Code = (int) HttpStatusCode.FailedDependency,
                Message = FailedDependencyErrorResponseMessage
            };

        public static BaseResponse<T> BadRequestResponse<T>(string? message = null) =>
            new BaseResponse<T>
            {
                Code = (int) HttpStatusCode.BadRequest,
                Message = message ?? "Bad request"
            };
    }

    public static class SuccessResponse
    {
        public static BaseResponse<T> OkResponse<T>(T data, string? message = null) =>
            new BaseResponse<T>
            {
                Code = (int) HttpStatusCode.OK,
                Message = message ?? DefaultOkResponseMessage,
                Data = data
            };
    }
}