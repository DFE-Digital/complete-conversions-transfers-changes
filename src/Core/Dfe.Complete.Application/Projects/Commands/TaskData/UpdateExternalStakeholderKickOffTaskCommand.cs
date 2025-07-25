using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateExternalStakeholderKickOffTaskCommand(ProjectId ProjectId, 
        bool? StakeholderKickOffIntroductoryEmails, 
        bool? LocalAuthorityProforma, 
        bool? CheckProvisionalDate, 
        bool? StakeholderKickOffSetupMeeting, 
        bool? StakeholderKickOffMeeting, 
        DateOnly? SignificantDate,
        string? UserEmail) : IRequest;

    public class UpdateExternalStakeholderKickOffTaskHandler(
        ICompleteRepository<Project> projectRepository,
        ICompleteRepository<ConversionTasksData> conversionTaskDataRepository,
        ICompleteRepository<TransferTasksData> transferTaskDataRepository,
        ICompleteRepository<User> userRepository,
        ICompleteRepository<SignificantDateHistoryReason> significantDateReasonHistoryRepository
        )
        : IRequestHandler<UpdateExternalStakeholderKickOffTaskCommand>
    {
        public async Task Handle(UpdateExternalStakeholderKickOffTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.Query().FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);
            
            if (project == null)
            {
                throw new NotFoundException("Project not found");
            }
            
            var user = await userRepository.FindAsync(u => u.Email == request.UserEmail, cancellationToken);
            if (user is null)
            {
                throw new NotFoundException("User not found", "email");
            }

            var isConversion = project.Type == ProjectType.Conversion;
            var projectTypeString = isConversion ? "Conversion" : "Transfer";

            if (isConversion)
            {
                var conversionTasksData = await conversionTaskDataRepository.Query().FirstOrDefaultAsync(x => x.Id == project.TasksDataId, cancellationToken);

                conversionTasksData!.StakeholderKickOffIntroductoryEmails = request.StakeholderKickOffIntroductoryEmails;
                conversionTasksData!.StakeholderKickOffLocalAuthorityProforma = request.LocalAuthorityProforma;
                conversionTasksData.StakeholderKickOffCheckProvisionalConversionDate = request.CheckProvisionalDate;
                conversionTasksData!.StakeholderKickOffSetupMeeting = request.StakeholderKickOffSetupMeeting;
                conversionTasksData!.StakeholderKickOffMeeting = request.StakeholderKickOffMeeting;
            }
            else
            {
                var transferTasksData = await transferTaskDataRepository.Query().FirstOrDefaultAsync(x => x.Id == project.TasksDataId, cancellationToken);
            
                transferTasksData!.StakeholderKickOffIntroductoryEmails = request.StakeholderKickOffIntroductoryEmails;
                transferTasksData!.StakeholderKickOffSetupMeeting = request.StakeholderKickOffSetupMeeting;
                transferTasksData!.StakeholderKickOffMeeting = request.StakeholderKickOffMeeting;
            }
            
            
            if (request.SignificantDate.HasValue)
            {
                var now = DateTime.UtcNow;

                var significantDateHistory = new SignificantDateHistory
                {
                    Id = new SignificantDateHistoryId(Guid.NewGuid()),
                    CreatedAt = now,
                    UpdatedAt = now,
                    ProjectId = project.Id,
                    UserId = user.Id,
                    PreviousDate = project.SignificantDate,
                    RevisedDate = request.SignificantDate,
                };
            
                var significantDateHistoryReason = new SignificantDateHistoryReason()
                {
                    Id = new SignificantDateHistoryReasonId(Guid.NewGuid()),
                    CreatedAt = now,
                    UpdatedAt = now,
                    SignificantDateHistoryId = significantDateHistory.Id,
                    ReasonType = SignificantDateReason.StakeholderKickOff.ToDescription()
                };


                var note = new Note()
                {
                    CreatedAt = now,
                    UpdatedAt = now,
                    Body = $"{projectTypeString} date confirmed as part of the External stakeholder kick off task.",
                    ProjectId = project.Id,
                    TaskIdentifier = significantDateHistoryReason.Id.Value.ToString(),
                    UserId = user.Id,
                    NotableId = significantDateHistoryReason.Id.Value,
                    NotableType = "SignificantDateHistoryReason"
                };
                
                project.UpdateSignificantDate(request.SignificantDate.Value);
                project.SignificantDateProvisional = false;
                
                project.AddSignificantDateHistory(significantDateHistory);
                await significantDateReasonHistoryRepository.AddAsync(significantDateHistoryReason, cancellationToken);
                
                project.AddNote(note);
            }

            await projectRepository.UpdateAsync(project, cancellationToken);
        }
    }
}