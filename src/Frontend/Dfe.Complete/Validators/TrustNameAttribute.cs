using Dfe.Complete.Application.Projects.Queries.GetProject;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Validators;

[AttributeUsage(AttributeTargets.Property)]
public partial class TrustNameAttribute : ValidationAttribute
{
    private readonly string _trustReference;
    private readonly ISender _sender;

    public TrustNameAttribute(string trustReference, ISender sender)
    {
        _trustReference = trustReference;
        _sender = sender;
    }
    
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var trustName = value as string ?? string.Empty;
        var trnProperty = validationContext.ObjectType.GetProperty(_trustReference);

        if (trnProperty == null)
        {
            return new ValidationResult($"Property '{_trustReference}' not found.");
        }

        var trn = trnProperty.GetValue(validationContext.ObjectInstance)?.ToString();

        if (string.IsNullOrEmpty(trustName) && string.IsNullOrEmpty(trn))
        {
            return new ValidationResult("Enter a trust name.");
        }

        if (string.IsNullOrEmpty(trustName))
        {
            return ValidationResult.Success;
        }

        return ValidateTrustNameAsync(trn, trustName, _sender).GetAwaiter().GetResult();
    }
    
    private async Task<ValidationResult?> ValidateTrustNameAsync(string? trn, string trustName, ISender sender)
    {
        if (string.IsNullOrEmpty(trn)) return ValidationResult.Success;

        try
        {
            var result = await sender.Send(new GetProjectByTrnQuery(trn));
            if (!result.IsSuccess) throw new NotFoundException(result.Error);

            var existingProject = result.Value;
            if (existingProject != null && existingProject.NewTrustName != trustName)
            {
                return new ValidationResult(
                    $"A trust with this TRN already exists. It is called {existingProject.NewTrustName}. Check the trust name you have entered for this conversion/transfer.");
            }

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