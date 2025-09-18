using Dfe.Complete.Constants;
using Dfe.Complete.Models.ExternalContact;
using FluentValidation;

namespace Dfe.Complete.Validators
{
    public class ExternalContactInputValidator<T>:  AbstractValidator<T> where T : ExternalContactInputModel
    {
        public ExternalContactInputValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage(ValidationConstants.FullNameRequiredMessage);

            RuleFor(x => x.Email)
           .EmailAddress()
           .Unless(x => string.IsNullOrEmpty(x.Email))
           .WithMessage(ValidationConstants.InvalidEmailMessage);

            RuleFor(x => x.Phone).Matches(ValidationExpressions.UKPhone)
            .Unless(x => string.IsNullOrEmpty(x.Phone))
            .WithMessage(ValidationConstants.NotRecognisedUKPhone);
        }
    }
}
