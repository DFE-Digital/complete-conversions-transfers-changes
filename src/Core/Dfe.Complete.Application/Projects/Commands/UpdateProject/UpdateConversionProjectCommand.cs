using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    public record UpdateConversionProjectCommand(
        ProjectId ProjectId,
        Ukprn? IncomingTrustUkprn,
        string? NewTrustReferenceNumber,
        string? GroupReferenceNumber,
        DateOnly AdvisoryBoardDate,
        string? AdvisoryBoardConditions,
        string EstablishmentSharepointLink,
        string IncomingTrustSharepointLink,
        bool IsHandingToRCS,
        bool DirectiveAcademyOrder,
        bool TwoRequiresImprovement,
        UserDto User
    ) : IRequest, IUpdateProjectRequest;

    public class UpdateConversionProjectCommandHandler : UpdateProjectCommandBase<UpdateConversionProjectCommand>, IRequestHandler<UpdateConversionProjectCommand>
    {
        public UpdateConversionProjectCommandHandler(
            ICompleteRepository<Project> projectRepository,
            ICompleteRepository<ProjectGroup> projectGroupRepository)
            : base(projectRepository, projectGroupRepository)
        {
        }

        protected override async Task UpdateSpecificProjectProperties(Project project, UpdateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            // Conversion-specific properties
            project.DirectiveAcademyOrder = request.DirectiveAcademyOrder;

            await Task.CompletedTask; // No async operations needed for conversion-specific updates
        }
    }
}
