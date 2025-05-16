using Dfe.Complete.Application.Common.Exceptions;
using Dfe.Complete.Logging.Middleware;
using Dfe.Complete.Middleware;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Dfe.Complete.Tests.Middleware
{

    public class ExceptionHandlerMiddlewareTests
    {
        private readonly Mock<ILogger<ExceptionHandlerMiddleware>> _loggerMock;
        private readonly DefaultHttpContext _httpContext;

        public ExceptionHandlerMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public async Task InvokeAsync_Should_Process_Request_Normally()
        {
            var middleware = new ExceptionHandlerMiddleware(context => Task.CompletedTask, _loggerMock.Object);

            await middleware.InvokeAsync(_httpContext);

            Assert.Equal(200, _httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task InvokeAsync_Should_Handle_Unauthorized_Response()
        {
            var middleware = new ExceptionHandlerMiddleware(context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Task.CompletedTask;
            }, _loggerMock.Object);

            var responseStream = new MemoryStream();
            _httpContext.Response.Body = responseStream;

            await middleware.InvokeAsync(_httpContext);

            responseStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseStream).ReadToEndAsync();

            Assert.Contains("You are not authorized", responseBody);
        }

        [Fact]
        public async Task InvokeAsync_Should_Handle_Forbidden_Response()
        {
            var middleware = new ExceptionHandlerMiddleware(context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Task.CompletedTask;
            }, _loggerMock.Object);

            var responseStream = new MemoryStream();
            _httpContext.Response.Body = responseStream;

            await middleware.InvokeAsync(_httpContext);

            responseStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseStream).ReadToEndAsync();

            Assert.Contains("You do not have permission", responseBody);
        }

        [Fact]
        public async Task InvokeAsync_Should_Handle_ValidationException()
        {
            var middleware = new ExceptionHandlerMiddleware(context =>
            {
                throw new ValidationException(
                 [
                    new ValidationFailure("Field", "Error message")
                 ]);
            }, _loggerMock.Object);

            var responseStream = new MemoryStream();
            _httpContext.Response.Body = responseStream;

            await middleware.InvokeAsync(_httpContext);

            Assert.Equal(400, _httpContext.Response.StatusCode);
            responseStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
            Assert.Contains("Field", responseBody);
        }

        [Fact]
        public async Task InvokeAsync_Should_Handle_General_Exception()
        {
            var middleware = new ExceptionHandlerMiddleware(context =>
            {
                throw new Exception("Something went wrong");
            }, _loggerMock.Object);

            var responseStream = new MemoryStream();
            _httpContext.Response.Body = responseStream;

            await middleware.InvokeAsync(_httpContext);

            Assert.Equal(500, _httpContext.Response.StatusCode);
            responseStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseStream).ReadToEndAsync();
            Assert.Contains("Internal Server Error", responseBody);
        }
    }
}
