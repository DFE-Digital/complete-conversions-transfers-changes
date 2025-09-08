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

            When(x => x.IsPrimaryProjectContact, () => {
                RuleFor(customer => customer.SelectedExternalContactType).Must(type => 
                    type == ExternalContactType.IncomingTrust.ToDescription() || 
                    type == ExternalContactType.OutgoingTrust.ToDescription() || 
                    type == ExternalContactType.SchoolOrAcademy.ToDescription()
                ).WithMessage("Only the incoming trust, outgoing trust and school or academy categories can have a primary contact.");
            });
        }
    }
}
