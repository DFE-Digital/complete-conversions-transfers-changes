using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateSignificantDateCommand(
    ProjectId ProjectId,
    DateOnly SignificantDate,
    Dictionary<SignificantDateReason, string> ReasonNotes,
    string UserEmail
    ) : IRequest;

public class UpdateSignificantDateCommandHandler(
    ICompleteRepository<Project> projectRepository,
    ICompleteRepository<User> userRepository,
    ICompleteRepository<SignificantDateHistoryReason> significantDateHistoryRepository
    )
    : IRequestHandler<UpdateSignificantDateCommand>
{
    public async Task Handle(UpdateSignificantDateCommand request, CancellationToken cancellationToken)
    {
        var project = await projectRepository.FindAsync(p => p.Id == request.ProjectId, cancellationToken);
        var user = await userRepository.FindAsync(u => u.Email == request.UserEmail, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User not found", "email");
        }
        
        var now = DateTime.UtcNow;

        var newDateHistory = new SignificantDateHistory
        {
            Id = new SignificantDateHistoryId(Guid.NewGuid()),
            CreatedAt = now,
            UpdatedAt = now,
            ProjectId = project.Id,
            UserId = user.Id,
            PreviousDate = project.SignificantDate,
            RevisedDate = request.SignificantDate,
        };
        
        List<SignificantDateHistoryReason> significantDateHistoryReasons = new List<SignificantDateHistoryReason>();
        List<Note> notes = new List<Note>();
        
        foreach (var reason in request.ReasonNotes)
        {
            var historyReason = new SignificantDateHistoryReason();
            historyReason.Id = new SignificantDateHistoryReasonId(Guid.NewGuid());
            historyReason.CreatedAt = now;
            historyReason.UpdatedAt = now;
            historyReason.ReasonType = reason.Key.ToDescription();
            historyReason.SignificantDateHistoryId = newDateHistory.Id;

            var note = new Note();
            note.CreatedAt = now;
            note.UpdatedAt = now;
            note.Body = reason.Value;
            note.ProjectId = project.Id;
            note.TaskIdentifier = historyReason.Id.Value.ToString();
            note.UserId = user.Id;
            note.NotableId = historyReason.Id.Value;
            note.NotableType = "SignificantDateHistoryReason";
            
            significantDateHistoryReasons.Add(historyReason);
            notes.Add(note);
        }
        
        project.UpdateSignificantDate(request.SignificantDate);
        project.AddSignificantDateHistory(newDateHistory);
        await significantDateHistoryRepository.AddRangeAsync(significantDateHistoryReasons, cancellationToken);

        foreach (var note in notes)
        {
            project.AddNote(note);
        }

        await projectRepository.UpdateAsync(project, cancellationToken);
    }
}