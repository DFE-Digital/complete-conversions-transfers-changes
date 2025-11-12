using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using ValidationException = Dfe.Complete.Application.Common.Exceptions.ValidationException;

namespace Dfe.Complete.Application.Common.Behaviours;

[ExcludeFromCodeCoverage]
public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count > 0)
        .SelectMany(r => r.Errors)
        .ToList();

        if (failures.Count > 0)
        {
            var errorMessages = string.Join(",", failures.Select(x => x.ErrorMessage));
            logger.LogError("Validation failures occurred in {HandlerName}. Request: {@Request}, Errors: {ErrorMessages}", nameof(ValidationBehaviour<TRequest, TResponse>), request, errorMessages);
            throw new ValidationException(failures);
        }
        return await next();
    }
}
