using Dfe.Complete.Application.Projects.Common;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject
{
    public record CreateConversionProjectCommand(
        Urn Urn,
        DateOnly SignificantDate,
        bool IsSignificantDateProvisional,
        Ukprn IncomingTrustUkprn,
        bool IsDueTo2Ri,
        bool HasAcademyOrderBeenIssued,
        DateOnly AdvisoryBoardDate,
        string AdvisoryBoardConditions,
        string EstablishmentSharepointLink,
        string IncomingTrustSharepointLink,
        string? GroupReferenceNumber,
        bool HandingOverToRegionalCaseworkService,
        string HandoverComments,
        string? UserAdId) : IRequest<ProjectId>;

    [Obsolete("Deprecated as in-app project creations are no longer required")] //NOSONAR
    public class CreateConversionProjectCommandHandler(
            ICompleteRepository<Project> projectRepository,
            ICompleteRepository<ConversionTasksData> conversionTaskRepository,
            ICreateProjectCommon createProjectCommon)
            : IRequestHandler<CreateConversionProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var commonProjectCommand = new CreateProjectCommonCommand(request.Urn, request.GroupReferenceNumber,
                request.HandingOverToRegionalCaseworkService, request.UserAdId);
            var commonProject = await createProjectCommon.CreateCommonProject(commonProjectCommand,
                cancellationToken);

            var conversionTaskId = Guid.NewGuid();
            var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), commonProject.CreatedAt, commonProject.CreatedAt);

            var project = Project.CreateConversionProject(
                commonProject.ProjectId,
                request.Urn,
                commonProject.CreatedAt,
                commonProject.CreatedAt,
                TaskType.Conversion,
                ProjectType.Conversion,
                conversionTaskId,
                request.SignificantDate,
                request.IsSignificantDateProvisional,
                request.IncomingTrustUkprn,
                commonProject.Region,
                request.IsDueTo2Ri,
                request.HasAcademyOrderBeenIssued,
                request.AdvisoryBoardDate,
                request.AdvisoryBoardConditions,
                request.EstablishmentSharepointLink,
                request.IncomingTrustSharepointLink,
                commonProject.ProjectGroupDto?.Id,
                commonProject.ProjectTeam,
                commonProject.CreatedByUser.Id,
                commonProject.AssignedUser?.Id,
                commonProject.AssignedAt,
                request.HandoverComments,
                commonProject.LocalAuthority.LocalAuthorityId.Value);

            await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
            await projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
    }
}