using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Validators;

public class UrnAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
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

        var establishmentsV4Client =
            (IEstablishmentsV4Client)validationContext.GetService(typeof(IEstablishmentsV4Client));
        
        var sender = (ISender)validationContext.GetService(typeof(ISender));

        try
        {
            establishmentsV4Client?.GetEstablishmentByUrnAsync(urn);
            
            var getProjectByUrnQueryResult = sender?.Send(new GetProjectByUrnQuery(new Urn(urn.ToInt()))).Result;

            if (!getProjectByUrnQueryResult.IsSuccess)
                throw new NotFoundException(getProjectByUrnQueryResult.Error);
            
            if (getProjectByUrnQueryResult.Value != null)
                return new ValidationResult($"A project with the urn: {urn} already exists");
        }
        catch (AggregateException ex)
        {
            return new ValidationResult("There's no school or academy with that URN. Check the number you entered is correct.");
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

        return ValidationResult.Success;
    }
}