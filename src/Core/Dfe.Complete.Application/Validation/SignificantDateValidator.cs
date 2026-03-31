using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Validation
{
    public interface ISignificantDateValidator
    {
        ValidationResult ValidateSignificantDate(DateOnly? significantDate, Project? existingProject = null);
        ValidationResult ValidateSignificantDateNotSameAsCurrent(DateOnly? significantDate, Project project);
        ValidationResult ValidateSignificantDateInFuture(DateOnly? significantDate);
    }

    public class SignificantDateValidator : ISignificantDateValidator
    {
        public ValidationResult ValidateSignificantDate(DateOnly? significantDate, Project? existingProject = null)
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

            return ValidationResult.Success();
        }

        public ValidationResult ValidateSignificantDateNotSameAsCurrent(DateOnly? significantDate, Project project)
        {
            if (significantDate.HasValue && significantDate == project.SignificantDate)
            {
                return ValidationResult.Error("The new date cannot be the same as the current date. Check you have entered the correct date.");
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
}