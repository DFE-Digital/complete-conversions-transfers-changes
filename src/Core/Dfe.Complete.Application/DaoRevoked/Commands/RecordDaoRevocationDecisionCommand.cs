using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.DaoRevoked.Interfaces;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.DaoRevoked.Commands
{
    public record RecordDaoRevocationDecisionCommand(ProjectId ProjectId, string DecisionMakerRole = "Minister") : IRequest<Result<bool>>
    {
        public UserId UserId { get; set; } = null!;
        public Dictionary<DaoRevokedReason, string> ReasonNotes { get; set; } = [];
        public string MinisterName { get; set; } = null!;
        public DateOnly? DecisionDate { get; set; } = null!;
    }

    public class RecordDaoRevocationDecisionCommandHandler(
        IProjectReadRepository projectReadRepository,
        IProjectWriteRepository projectWriteRepository,
        INoteWriteRepository noteWriteRepository,  
        IDaoRevocationWriteRepository daoRevocationWriteRepository,
        ILogger<RecordDaoRevocationDecisionCommandHandler> logger)
        : IRequestHandler<RecordDaoRevocationDecisionCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RecordDaoRevocationDecisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var project = await projectReadRepository.Projects.FirstOrDefaultAsync(u => u.Id == request.ProjectId, cancellationToken)
                    ?? throw new NotFoundException($"Project {request.ProjectId} not found");
                var now = DateTime.UtcNow;

                var daoRevocation = new DaoRevocation
                {
                    Id = new DaoRevocationId(Guid.NewGuid()),
                    DateOfDecision = request.DecisionDate,
                    DecisionMakersName = request.MinisterName,
                    ProjectId = request.ProjectId,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await daoRevocationWriteRepository.CreateDaoRevocationAsync(daoRevocation, cancellationToken);

                await AddDaoRevocationReason(request, daoRevocation.Id, now, cancellationToken);

                UpdateProjectWithDaoRevoked(project, now);
                await projectWriteRepository.UpdateProjectAsync(project, cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(RecordDaoRevocationDecisionCommandHandler), request);
                return Result<bool>.Failure(ex.Message);
            }
        }
        private static void UpdateProjectWithDaoRevoked(Project project, DateTime now)
        {
            project.State = ProjectState.DaoRevoked;
            project.UpdatedAt = now;
        }

        private async Task AddDaoRevocationReason(RecordDaoRevocationDecisionCommand request, DaoRevocationId daoRevocationId, DateTime creationTime, CancellationToken cancellationToken)
        {
            foreach (var reason in request.ReasonNotes)
            {
                var daoRevocationReason = new DaoRevocationReason
                {
                    ReasonType = reason.Key.ToDescription(),
                    DaoRevocationId = daoRevocationId,
                    Id = new DaoRevocationReasonId(Guid.NewGuid())
                };
                await daoRevocationWriteRepository.CreateDaoRevocationReasonAsync(daoRevocationReason, cancellationToken);

                await noteWriteRepository.CreateNoteAsync(new Note
                {
                    Id = new NoteId(Guid.NewGuid()),
                    ProjectId = request.ProjectId,
                    UserId = request.UserId,
                    Body = reason.Value,
                    NotableId = daoRevocationReason.Id.Value,
                    NotableType = NotableType.DaoRevocationReason.ToDescription(),
                    CreatedAt = creationTime,
                    UpdatedAt = creationTime
                }, cancellationToken);
            }
        }
    }
}
