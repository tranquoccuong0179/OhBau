using OhBau.Model.Payload.Response;
using System.Net;
using System.Text.Json;
using OhBau.Model.Exception;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace OhBau.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            BaseResponse<object> errorResponse;

            switch (exception)
            {
                case ModelValidationException modelValidationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new BaseResponse<object>
                    {
                        status = StatusCodes.Status400BadRequest.ToString(),
                        message = $"[{string.Join(", ", modelValidationEx.Errors.SelectMany(kvp => kvp.Value))}]",
                        data = null
                    };
                    _logger.LogInformation("Validation error model: {Errors}", JsonSerializer.Serialize(modelValidationEx.Errors));
                    break;

                case CustomValidationException validationEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new BaseResponse<object>
                    {
                        status = StatusCodes.Status400BadRequest.ToString(),
                        message = string.Join("; ", validationEx.Errors),
                        data = null
                    };
                    _logger.LogInformation("Custom validation error: {Errors}", JsonSerializer.Serialize(validationEx.Errors));
                    break;

                case BadHttpRequestException badRequestEx:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new BaseResponse<object>
                    {
                        status = HttpStatusCode.BadRequest.ToString(),
                        message = badRequestEx.Message,
                        data = null
                    };
                    _logger.LogInformation(exception.Message);
                    break;
                case NotFoundException notFoundEx:
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse = new BaseResponse<object>
                    {
                        status = StatusCodes.Status404NotFound.ToString(),
                        message = notFoundEx.Message,
                        data = null
                    };
                    _logger.LogWarning("Not found error: {Message}", notFoundEx.Message);
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse = new BaseResponse<object>
                    {
                        status = HttpStatusCode.InternalServerError.ToString(),
                        message = "Lỗi hệ thống: " + exception.Message,
                        data = null
                    };
                    _logger.LogError(exception.ToString());
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);
        }
    }
}
