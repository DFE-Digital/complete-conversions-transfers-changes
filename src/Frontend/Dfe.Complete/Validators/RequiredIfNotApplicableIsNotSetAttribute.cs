using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Validators
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RequiredIfNotApplicableIsNotSetAttribute(string dependentProperty) : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(dependentProperty);
            if (property == null)
            {
                return new ValidationResult($"Unknown property: {dependentProperty}");
            }

            var dependentValue = property.GetValue(validationContext.ObjectInstance, null) as bool?;

            if (dependentValue != true && string.IsNullOrWhiteSpace(value as string))
            {
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} is required when {dependentProperty} is not applicable.");
            }

            return ValidationResult.Success;
        }
    }

}
