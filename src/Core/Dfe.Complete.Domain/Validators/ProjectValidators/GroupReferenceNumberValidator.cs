using FluentValidation;

namespace Dfe.Complete.Domain.Validators.Project
{
    public class GroupReferenceNumberValidator : AbstractValidator<string?>
    {
        public GroupReferenceNumberValidator()
        {
            RuleFor(grn => grn)
                .Matches(@"^GRP_\d{8}$")
                .When(grn => !string.IsNullOrEmpty(grn))
                .WithMessage("A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001");
        }
    }
}
