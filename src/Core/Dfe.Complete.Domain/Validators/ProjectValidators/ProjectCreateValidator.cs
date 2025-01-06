using FluentValidation;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Validators.Project;

namespace Dfe.Complete.Domain.Validators.ProjectValidators
{
    public class ProjectCreateValidator : AbstractValidator<Entities.Project>
    {
        public ProjectCreateValidator()
        {
            RuleFor(p => p.NewTrustReferenceNumber).SetValidator(new GroupReferenceNumberValidator());  
        }
    }
}
