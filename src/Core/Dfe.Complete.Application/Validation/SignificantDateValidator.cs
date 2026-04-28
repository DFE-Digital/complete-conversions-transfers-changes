using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Application.Validation;

public interface ISignificantDateValidator
{
    ValidationResult ValidateSignificantDate(DateOnly? significantDate, ProjectDto? existingProject = null, DateOnly? payrollDeadline = null);
    ValidationResult ValidatePayrollDeadline(DateOnly? payrollDeadline, ProjectDto project);
    ValidationResult ValidateSignificantDateInFuture(DateOnly? significantDate);
}

public class SignificantDateValidator : ISignificantDateValidator
{
    public ValidationResult ValidateSignificantDate(DateOnly? significantDate, ProjectDto? existingProject = null, DateOnly? payrollDeadline = null)
    {
        // Basic future date validation
        var futureValidation = ValidateSignificantDateInFuture(significantDate);
        if (!futureValidation.IsValid)
            return futureValidation;

        // If we have an existing project, check it's not the same as current date
        if (existingProject != null)
        {
            var sameAsCurrentValidation = ValidateSignificantDateNotSameAsCurrent(significantDate, existingProject);
            if (!sameAsCurrentValidation.IsValid)
                return sameAsCurrentValidation;
        }

        // Cross-model validation: significant date must be after payroll deadline (if payroll deadline is provided)
        if (payrollDeadline.HasValue)
        {
            var afterPayrollValidation = ValidateSignificantDateAfterPayrollDeadline(significantDate, payrollDeadline);
            if (!afterPayrollValidation.IsValid)
                return afterPayrollValidation;
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidateSignificantDateInFuture(DateOnly? significantDate)
    {
        if (significantDate.HasValue && significantDate.Value.ToDateTime(new TimeOnly()) < DateTime.Today)
        {
            return ValidationResult.Error("The Significant date must be in the future.");
        }

        return ValidationResult.Success();
    }

    private static ValidationResult ValidateSignificantDateAfterPayrollDeadline(DateOnly? significantDate, DateOnly? payrollDeadline)
    {
        if (significantDate.HasValue && payrollDeadline.HasValue && significantDate <= payrollDeadline.Value)
        {
            return ValidationResult.Error("The significant date must be after the payroll deadline.");
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePayrollDeadline(DateOnly? payrollDeadline, ProjectDto project)
    {
        var futureValidation = ValidatePayrollDeadlineInFuture(payrollDeadline);
        if (!futureValidation.IsValid)
            return futureValidation;

        if (project.SignificantDateProvisional != true)
        {
            var beforeSignificantDateValidation = ValidatePayrollDeadlineBeforeSignificantDate(payrollDeadline, project.SignificantDate);
            if (!beforeSignificantDateValidation.IsValid)
                return beforeSignificantDateValidation;
        }

        return ValidationResult.Success();
    }

    private static ValidationResult ValidatePayrollDeadlineInFuture(DateOnly? payrollDeadline)
    {
        if (payrollDeadline.HasValue && payrollDeadline.Value.ToDateTime(new TimeOnly()) < DateTime.Today)
        {
            return ValidationResult.Error("The payroll deadline must be in the future.");
        }

        return ValidationResult.Success();
    }

    private static ValidationResult ValidatePayrollDeadlineBeforeSignificantDate(DateOnly? payrollDeadline, DateOnly? significantDate)
    {
        if (payrollDeadline.HasValue && significantDate.HasValue && payrollDeadline >= significantDate.Value)
        {
            return ValidationResult.Error("The payroll deadline must be before the significant date.");
        }

        return ValidationResult.Success();
    }

    private static ValidationResult ValidateSignificantDateNotSameAsCurrent(DateOnly? significantDate, ProjectDto project)
    {
        if (significantDate.HasValue && significantDate == project.SignificantDate)
        {
            return ValidationResult.Error("The new date cannot be the same as the current date. Check you have entered the correct date.");
        }

        return ValidationResult.Success();
    }

}

public class ValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }

    private ValidationResult(bool isValid, string? errorMessage = null)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true);
    public static ValidationResult Error(string errorMessage) => new(false, errorMessage);
}
