using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class UrnAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        try
        {
            // Fetch the display name if it is provided
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName);
            var displayAttribute = property?.GetCustomAttribute<DisplayAttribute>();
            var displayName = displayAttribute?.GetName() ?? validationContext.DisplayName;

            var urn = value as string;

            if (string.IsNullOrEmpty(urn))
                return ValidationResult.Success;

            if (urn.Length != 6)
                return new ValidationResult($"The {displayName} must be 6 digits long. For example, 123456.");

            var sender = (ISender)validationContext.GetService(typeof(ISender));

            var result = sender.Send(new GetProjectByUrnQuery(new Urn(urn.ToInt())));

            if (!result.Result.IsSuccess)
                throw new NotFoundException(result.Result.Error);

            if (result.Result?.Value != null)
                return new ValidationResult($"A project with the urn: {urn} already exists");

            // If valid, return success
            return ValidationResult.Success;
        }
        catch (NotFoundException notFoundException)
        {
            return new ValidationResult(notFoundException.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}