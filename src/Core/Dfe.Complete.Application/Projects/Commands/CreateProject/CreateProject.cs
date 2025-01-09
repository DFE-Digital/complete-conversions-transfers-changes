using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Models;
using Dfe.Complete.Utils;

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
        string GroupReferenceNumber,
        bool HandingOverToRegionalCaseworkService,
        string HandoverComments,
        string? UserAdId) : IRequest<ProjectId>;

    public class CreateConversionProjectCommandHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<ConversionTasksData> conversionTaskRepository)
        : IRequestHandler<CreateConversionProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var user = await projectRepository.GetUserByAdId(request.UserAdId, cancellationToken);
            var userTeam = user?.Team;
            
            var projectTeam = EnumExtensions.FromDescription<ProjectTeam>(userTeam);
            var region = EnumMapper.MapTeamToRegion(projectTeam);
            var regionCharValue = region.GetCharValue();
            
            var createdAt = DateTime.UtcNow;
            var conversionTaskId = Guid.NewGuid();
            var projectId = new ProjectId(Guid.NewGuid());

            var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), createdAt, createdAt);

            var groupId =
                await projectRepository.GetProjectGroupIdByIdentifierAsync(request.GroupReferenceNumber,
                    cancellationToken);

            string team;
            DateTime? assignedAt = null;
            User? assignedTo = null;

            if (request.HandingOverToRegionalCaseworkService)
            {
                team = "regional_casework_services";
            }
            else
            {
                team = projectTeam.ToDescription();
                assignedAt = DateTime.UtcNow;
                assignedTo = user;
            }

            var project = Project.CreateConversionProject(
                projectId,
                request.Urn,
                createdAt,
                createdAt,
                TaskType.Conversion,
                ProjectType.Conversion,
                conversionTaskId,
                request.SignificantDate,
                request.IsSignificantDateProvisional,
                request.IncomingTrustUkprn,
                regionCharValue,
                request.IsDueTo2Ri,
                request.HasAcademyOrderBeenIssued,
                request.AdvisoryBoardDate,
                request.AdvisoryBoardConditions,
                request.EstablishmentSharepointLink,
                request.IncomingTrustSharepointLink,
                groupId?.Value,
                team,
                user?.Id,
                assignedTo,
                assignedAt); 
            
            if (!string.IsNullOrEmpty(request.HandoverComments))
            {
                project.Notes.Add(new Note
                {
                    Id = new NoteId(Guid.NewGuid()), CreatedAt = createdAt, Body = request.HandoverComments,
                    ProjectId = projectId, TaskIdentifier = "handover", UserId = user?.Id
                });
            }
            
            await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
            await projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
    }
}