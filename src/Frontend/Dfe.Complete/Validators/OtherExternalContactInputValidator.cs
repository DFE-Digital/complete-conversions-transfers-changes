using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Utils;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using DocumentFormat.OpenXml.Wordprocessing;
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

            When(
                    x => x.SelectedExternalContactType == ExternalContactType.Solicitor.ToDescription()
                    || x.SelectedExternalContactType == ExternalContactType.Diocese.ToDescription()
                    || x.SelectedExternalContactType == ExternalContactType.Other.ToDescription(), 
                    () => {
                    RuleFor(x => x.IsPrimaryProjectContact).Equal(false).WithMessage("Only the incoming trust, outgoing trust, school or academy and local authority categories can have a primary contact.");
                    }
                );

            RuleFor(x => x.Email)
            .EmailAddress()
            .Unless(x => string.IsNullOrEmpty(x.Email))
            .WithMessage("Enter an email address in the correct format, like name@example.com");
        }
    }
}
