using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Services;
using Dfe.Complete.Utils;
using FluentValidation;

namespace Dfe.Complete.Validators
{
    public class OtherExternalContactInputValidator : AbstractValidator<OtherExternalContactInputModel>
    {
        public OtherExternalContactInputValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Enter a name");

            RuleFor(x => x.Role)
               .NotEmpty().WithMessage("Enter a role");

            When(x => x.SelectedExternalContactType == ExternalContactType.Solicitor.ToDescription() ||
                    x.SelectedExternalContactType == ExternalContactType.Diocese.ToDescription() ||
                    x.SelectedExternalContactType == ExternalContactType.Other.ToDescription(), () => {
                RuleFor(x => x.IsPrimaryProjectContact).Equal(false).WithMessage("Only the incoming trust, outgoing trust, school or academy and local authority categories can have a primary contact.");
            });
        }
    }
}
