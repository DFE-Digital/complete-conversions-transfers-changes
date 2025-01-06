using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Models;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject
{
    public record CreateConversionProjectCommand(
        Urn Urn,
        DateOnly SignificantDate,
        bool IsSignificantDateProvisional,
        Ukprn IncomingTrustUkprn,
        Region Region,
        bool IsDueTo2Ri,
        bool HasAcademyOrderBeenIssued,
        DateOnly AdvisoryBoardDate,
        string AdvisoryBoardConditions,
        string EstablishmentSharepointLink,
        string IncomingTrustSharepointLink,
        string GroupReferenceNumber,
        bool HandingOverToRegionalCaseworkService,
        string HandoverComments) : IRequest<ProjectId>;

    public class CreateConversionProjectCommandHandler(ICompleteRepository<Project> projectRepository,
        ICompleteRepository<ConversionTasksData> conversionTaskRepository)
        : IRequestHandler<CreateConversionProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(CreateConversionProjectCommand request, CancellationToken cancellationToken)
        {
            var createdAt = DateTime.UtcNow;
            var conversionTaskId = Guid.NewGuid();
            var projectId = new ProjectId(Guid.NewGuid());
            
            var conversionTask = new ConversionTasksData(new TaskDataId(conversionTaskId), createdAt, createdAt);
            
            
            var groupId =
                await projectRepository.GetProjectGroupIdByIdentifierAsync(request.GroupReferenceNumber,
                    cancellationToken);
            
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
                request.Region,
                request.IsDueTo2Ri, 
                request.HasAcademyOrderBeenIssued,
                request.AdvisoryBoardDate,
                request.AdvisoryBoardConditions,
                request.EstablishmentSharepointLink,
                request.IncomingTrustSharepointLink,
                groupId?.Value);

            if (!string.IsNullOrEmpty(request.HandoverComments))
            {
                project.Notes.Add(new Note
                {
                    Id = new NoteId(Guid.NewGuid()), CreatedAt = project.CreatedAt, Body = request.HandoverComments,
                    ProjectId = project.Id, TaskIdentifier = "handover", UserId = project.RegionalDeliveryOfficerId
                });
            }

            if (request.HandingOverToRegionalCaseworkService)
            {
                project.Team = "regional_casework_services";
            }
            else
            {
                project.Team = request.Region.ToString();
                project.AssignedAt = DateTime.UtcNow;
                project.AssignedTo = project.RegionalDeliveryOfficer;
            }
            
            await conversionTaskRepository.AddAsync(conversionTask, cancellationToken);
            await projectRepository.AddAsync(project, cancellationToken);

            return project.Id;
        }
    }
}