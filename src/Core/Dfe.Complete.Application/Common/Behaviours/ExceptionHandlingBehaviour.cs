using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Application.Common.Behaviours;

[ExcludeFromCodeCoverage]
public class ExceptionHandlingBehavior<TRequest, TResponse>(
    ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
#pragma warning disable S2139
        try
        {
            return await next();
        }
        catch (NotFoundException notFoundException)
        {
            var requestName = typeof(TRequest).Name;
            logger.LogError(notFoundException, 
                "Not Found Exception for Request: {Name} {@Request} with message: {Message}",
                requestName, 
                request,
                notFoundException.Message);
            throw;
        }
#pragma warning restore S2139
    }
}