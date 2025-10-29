using Dfe.Complete.Application.Common.Exceptions;
using Dfe.Complete.Logging.Models;
using Dfe.Complete.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using SystemValidationException = System.ComponentModel.DataAnnotations.ValidationException;

namespace Dfe.Complete.Logging.Middleware
{
    public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        private static string ContentTypeJson = "application/json";
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);

                // Check for 401 or 403 status codes after the request has been processed
                if (context.Response.StatusCode == (int)HttpStatusCode.Unauthorized)
                {
                    await HandleUnauthorizedResponseAsync(context);
                }
                else if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
                {
                    await HandleForbiddenResponseAsync(context);
                }
            }
            catch (UnprocessableContentException ex)
            {
                logger.LogInformation(ex, "Validation error: {Message}", ex.Message);
                await HandleUnprocessableContentException(context, ex);
            }
            catch (SystemValidationException ex)
            {
                logger.LogError(ex, "Validation error: {Message}", ex.Message);
                await HandleSystemValidationException(context, ex);
            }
            catch (ValidationException ex)
            {
                logger.LogError(ex, "Validation error: {Message}", ex.Message);
                await HandleValidationException(context, ex);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An exception occurred: {Message}. Stack Trace: {StackTrace}", ex.Message, ex.StackTrace);
                await HandleExceptionAsync(context, ex);
            }
        }

        // Handle unprocessable content exceptions
        private static async Task HandleUnprocessableContentException(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

            var errorResponse = new ErrorResponse
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = ex.Message
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        // Handle validation exceptions
        private static async Task HandleValidationException(HttpContext httpContext, Exception ex)
        {
            if (IsApiRequest(httpContext))
            {
                var exception = (ValidationException)ex;

                httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                await httpContext.Response.WriteAsJsonAsync(new ValidationProblemDetails(exception.Errors)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
                });
            }
            else
            {
                throw ex;
            }
        }

        private static async Task HandleSystemValidationException(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var errorResponse = new ErrorResponse
            {
                StatusCode = httpContext.Response.StatusCode,
                Message = ex.Message
            };

            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        // Handle 401 Unauthorized
        private async Task HandleUnauthorizedResponseAsync(HttpContext context)
        {
            logger.LogWarning("Unauthorized access attempt detected.");

            if (context.Response.HasStarted) return;
            context.Response.ContentType = ContentTypeJson;

            var errorResponse = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "You are not authorized to access this resource."
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        // Handle 403 Forbidden
        private async Task HandleForbiddenResponseAsync(HttpContext context)
        {
            logger.LogWarning("Forbidden access attempt detected.");
            context.Response.ContentType = ContentTypeJson;
            var errorResponse = new ErrorResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = "You do not have permission to access this resource."
            };
            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        // Handle general exceptions (500 Internal Server Error)
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            logger.LogError(exception, "Unhandled Exception: {Message}", exception.Message);
            if (IsApiRequest(context))
            {
                context.Response.ContentType = ContentTypeJson;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var errorResponse = new ErrorResponse
                {
                    StatusCode = context.Response.StatusCode,
                    Message = "Internal Server Error: " + exception.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            }
            else
            {
                throw exception;
            }
        }

        private static bool IsApiRequest(HttpContext context)
        {
            // Adjust this logic to match your API route conventions
            return context.Request.Path.StartsWithSegments("/api") ||
                context.Request.Headers.Accept.Any(h => h!.Contains(ContentTypeJson));
        }
    }
}
