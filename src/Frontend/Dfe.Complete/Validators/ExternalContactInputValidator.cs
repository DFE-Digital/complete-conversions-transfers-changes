using Dfe.Complete.Models.ExternalContact;
using FluentValidation;

namespace Dfe.Complete.Validators
{
    public class ExternalContactInputValidator:  AbstractValidator<ExternalContactInputModel>
    {
        public ExternalContactInputValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Enter a name");
        }
    }
}
