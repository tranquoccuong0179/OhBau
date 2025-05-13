using OhBau.Model.Payload.Response;
using System.Net;
using System.Text.Json;
using OhBau.Model.Exception;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using Microsoft.AspNetCore.Http.HttpResults;

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
                if (!context.Response.HasStarted)
                {
                    if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                    {
                        await HandleUnauthorizedAsync(context);
                    }
                    else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                    {
                        await HandleForbiddenAsync(context);
                    }
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleUnauthorizedAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

            var errorResponse = new BaseResponse<object>
            {
                status = StatusCodes.Status401Unauthorized.ToString(),
                message = "Không được phép truy cập: Vui lòng đăng nhập.",
                data = null
            };

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);

            _logger.LogWarning("Unauthorized access attempt.");
        }

        private async Task HandleForbiddenAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            var errorResponse = new BaseResponse<object>
            {
                status = StatusCodes.Status403Forbidden.ToString(),
                message = "Không có quyền truy cập: Bạn không có vai trò phù hợp.",
                data = null
            };

            var result = JsonSerializer.Serialize(errorResponse);
            await context.Response.WriteAsync(result);

            _logger.LogWarning("Forbidden access attempt.");
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
                case ForbiddentException forbiddentEx:
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse = new BaseResponse<object>
                    {
                        status = HttpStatusCode.Forbidden.ToString(),
                        message = forbiddentEx.Message,
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
                        message = "Lỗi hệ thống: " + exception.Message + exception.StackTrace + exception.ToString(),
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
