using Dfe.Complete.Application.Projects.Common;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

public record CreateTransferProjectCommand(
    Urn Urn,
    Ukprn OutgoingTrustUkprn,
    Ukprn IncomingTrustUkprn,
    DateOnly SignificantDate,
    bool IsSignificantDateProvisional,
    bool IsDueTo2Ri,
    bool IsDueToInedaquateOfstedRating,
    bool IsDueToIssues,
    bool OutGoingTrustWillClose,
    bool HandingOverToRegionalCaseworkService,
    DateOnly AdvisoryBoardDate,
    string AdvisoryBoardConditions,
    string EstablishmentSharepointLink,
    string IncomingTrustSharepointLink,
    string OutgoingTrustSharepointLink,
    string? GroupReferenceNumber,
    string HandoverComments,
    string? UserAdId) : IRequest<ProjectId>;

[Obsolete("Deprecated as in-app project creations are no longer required")]
public class CreateTransferProjectCommandHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<TransferTasksData> transferTaskRepository,
    ICreateProjectCommon createProjectCommon)
    : IRequestHandler<CreateTransferProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateTransferProjectCommand request, CancellationToken cancellationToken)
    {
        var commonProjectCommand = new CreateProjectCommonCommand(request.Urn, request.GroupReferenceNumber,
            request.HandingOverToRegionalCaseworkService, request.UserAdId);
        var commonProject = await createProjectCommon.CreateCommonProject(commonProjectCommand,
            cancellationToken);

        var tasksDataId = Guid.NewGuid();
        var transferTask = new TransferTasksData(new TaskDataId(tasksDataId), commonProject.CreatedAt, commonProject.CreatedAt, request.IsDueToInedaquateOfstedRating, request.IsDueToIssues, request.OutGoingTrustWillClose);


        var project = Project.CreateTransferProject
        (
            commonProject.ProjectId,
            request.Urn,
            commonProject.CreatedAt,
            commonProject.CreatedAt,
            TaskType.Transfer,
            ProjectType.Transfer,
            tasksDataId,
            commonProject.Region,
            commonProject.ProjectTeam,
            commonProject.CreatedByUser.Id,
            commonProject.AssignedUser?.Id,
            commonProject.AssignedAt,
            request.IncomingTrustUkprn,
            request.OutgoingTrustUkprn,
            commonProject.ProjectGroupDto?.Id,
            request.EstablishmentSharepointLink,
            request.IncomingTrustSharepointLink,
            request.OutgoingTrustSharepointLink,
            request.AdvisoryBoardDate,
            request.AdvisoryBoardConditions,
            request.SignificantDate,
            request.IsSignificantDateProvisional,
            request.IsDueTo2Ri,
            request.HandoverComments,
            commonProject.LocalAuthority.LocalAuthorityId.Value
        );

        await transferTaskRepository.AddAsync(transferTask, cancellationToken);
        await projectRepository.AddAsync(project, cancellationToken);

        return project.Id;
    }

}