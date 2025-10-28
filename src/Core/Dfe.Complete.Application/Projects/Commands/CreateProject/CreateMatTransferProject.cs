using Dfe.Complete.Application.Projects.Common;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

[Obsolete("Deprecated as in-app project creations are no longer required")] //NOSONAR
public record CreateMatTransferProjectCommand(
    Urn Urn,
    string NewTrustName,
    string NewTrustReferenceNumber,
    Ukprn OutgoingTrustUkprn,
    DateOnly SignificantDate,
    bool IsSignificantDateProvisional,
    bool IsDueTo2Ri,
    bool IsDueToInedaquateOfstedRating,
    bool IsDueToIssues,
    bool HandingOverToRegionalCaseworkService,
    bool OutGoingTrustWillClose,
    DateOnly AdvisoryBoardDate,
    string AdvisoryBoardConditions,
    string EstablishmentSharepointLink,
    string IncomingTrustSharepointLink,
    string OutgoingTrustSharepointLink,
    string HandoverComments,
    string? UserAdId) : IRequest<ProjectId>;

public class CreateMatTransferProjectCommandHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<TransferTasksData> transferTaskRepository,
        ICreateProjectCommon createProjectCommon)
        : IRequestHandler<CreateMatTransferProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateMatTransferProjectCommand request, CancellationToken cancellationToken)
        {
            var commonProjectCommand = new CreateProjectCommonCommand(request.Urn, null,
                request.HandingOverToRegionalCaseworkService, request.UserAdId);
            var commonProject = await createProjectCommon.CreateCommonProject(commonProjectCommand,
                cancellationToken);
            
            var tasksDataId = Guid.NewGuid();
            var transferTask = new TransferTasksData(new TaskDataId(tasksDataId), commonProject.CreatedAt, commonProject.CreatedAt, request.IsDueToInedaquateOfstedRating, request.IsDueToIssues, request.OutGoingTrustWillClose);

            var project = Project.CreateMatTransferProject(
                    commonProject.ProjectId,
                    request.Urn,
                    commonProject.CreatedAt,
                    updatedAt: commonProject.CreatedAt,
                    request.OutgoingTrustUkprn,
                    TaskType.Transfer,
                    ProjectType.Transfer,
                    tasksDataId,
                    commonProject.Region,
                    commonProject.ProjectTeam,
                    commonProject.CreatedByUser.Id,
                    commonProject.AssignedUser?.Id,
                    commonProject.AssignedAt,
                    request.EstablishmentSharepointLink,
                    request.IncomingTrustSharepointLink,
                    request.OutgoingTrustSharepointLink,
                    request.AdvisoryBoardDate,
                    request.AdvisoryBoardConditions,
                    request.SignificantDate,
                    request.IsSignificantDateProvisional,
                    request.IsDueTo2Ri,
                    request.NewTrustName,
                    request.NewTrustReferenceNumber,
                    request.HandoverComments,
                    commonProject.LocalAuthority.LocalAuthorityId.Value);
            
        await transferTaskRepository.AddAsync(transferTask, cancellationToken);
        await projectRepository.AddAsync(project, cancellationToken);

        return project.Id;
    }
}