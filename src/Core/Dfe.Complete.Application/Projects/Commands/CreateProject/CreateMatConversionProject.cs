using Dfe.Complete.Application.Projects.Common;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

public record CreateMatConversionProjectCommand(
    Urn Urn,
    string NewTrustName,
    string NewTrustReferenceNumber,
    DateOnly SignificantDate,
    bool IsSignificantDateProvisional,
    bool IsDueTo2Ri,
    bool HasAcademyOrderBeenIssued,
    DateOnly AdvisoryBoardDate,
    string AdvisoryBoardConditions,
    string EstablishmentSharepointLink,
    string IncomingTrustSharepointLink,
    bool HandingOverToRegionalCaseworkService,
    string? HandoverComments,
    string? UserAdId) : IRequest<ProjectId>;

public class CreateMatConversionProjectCommandHandler(
       ICompleteRepository<Project> projectRepository,
       ICompleteRepository<ConversionTasksData> conversionTaskRepository,
       ICreateProjectCommon createProjectCommon)
       : IRequestHandler<CreateMatConversionProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateMatConversionProjectCommand request, CancellationToken cancellationToken)
    {
        var commonProjectCommand = new CreateProjectCommonCommand(request.Urn, null,
            request.HandingOverToRegionalCaseworkService, request.UserAdId);
        var commonProject = await createProjectCommon.CreateCommonProject(commonProjectCommand,
            cancellationToken);

        var conversionTaskId = Guid.NewGuid();
        var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), commonProject.CreatedAt, commonProject.CreatedAt);

        var project = Project.CreateMatConversionProject(
            commonProject.ProjectId,
            request.Urn,
            commonProject.CreatedAt,
            updatedAt: commonProject.CreatedAt,
            TaskType.Conversion,
            ProjectType.Conversion,
            conversionTaskId,
            commonProject.Region,
            commonProject.ProjectTeam,
            commonProject.CreatedByUser.Id,
            commonProject.AssignedUser?.Id,
            commonProject.AssignedAt,
            request.EstablishmentSharepointLink,
            request.IncomingTrustSharepointLink,
            request.AdvisoryBoardDate,
            request.AdvisoryBoardConditions,
            request.SignificantDate,
            request.IsSignificantDateProvisional,
            request.IsDueTo2Ri,
            request.NewTrustName,
            request.NewTrustReferenceNumber,
            request.HasAcademyOrderBeenIssued,
            request.HandoverComments,
            commonProject.LocalAuthority.LocalAuthorityId.Value);

        await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
        await projectRepository.AddAsync(project, cancellationToken);

        return project.Id;
    }
}