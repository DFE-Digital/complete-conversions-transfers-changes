using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Utils;
using FluentValidation;

namespace Dfe.Complete.Validators
{
    public class OtherExternalContactInputValidator : ExternalContactInputValidator<OtherExternalContactInputModel>
    {
        public OtherExternalContactInputValidator()
        { 

            RuleFor(x => x.Role)
               .NotEmpty().WithMessage(ValidationConstants.RoleRequiredMessage);

            When(
                    x => x.SelectedExternalContactType == ExternalContactType.Solicitor.ToDescription()
                    || x.SelectedExternalContactType == ExternalContactType.Diocese.ToDescription()
                    || x.SelectedExternalContactType == ExternalContactType.Other.ToDescription(), 
                    () => {
                    RuleFor(x => x.IsPrimaryProjectContact).Equal(false).WithMessage(ValidationConstants.InvalidPrimaryContactMessage);
                    }
                );         
        }
    }
}
