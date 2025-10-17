using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Domain.Validators;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class ProjectTypeAttribute : ValidationAttribute
{
    public ProjectTypeAttribute() => ErrorMessage = ValidationConstants.ProjectTypeValidationMessage;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        if (((ProjectType)value != ProjectType.Conversion && (ProjectType)value != ProjectType.Transfer))
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}

