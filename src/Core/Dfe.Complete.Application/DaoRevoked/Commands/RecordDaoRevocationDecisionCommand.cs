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

namespace Dfe.Complete.Application.DaoRevoked.Commands
{
    public record RecordDaoRevocationDecisionCommand(ProjectId ProjectId, string DecisionMakerRole = "Minister") : IRequest<Result<bool>>
    {
        public UserId UserId { get; set; } = null!;
        public Dictionary<DaoRevokedReason, string> ReasonNotes { get; set; } = [];
        public string MinisterName { get; set; } = null!;
        public DateOnly? DecisionDate { get; set; } = null!;
    }

    public class AddDaoRevokedDecisionCommandHandler(
        IProjectReadRepository projectReadRepository,
        IProjectWriteRepository projectWriteRepository,
        INoteWriteRepository noteWriteRepository,  
        IDaoRevocationWriteRepository daoRevocationWriteRepository)
        : IRequestHandler<RecordDaoRevocationDecisionCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RecordDaoRevocationDecisionCommand request, CancellationToken cancellationToken)
        {
            var project = await projectReadRepository.ProjectsNoIncludes.FirstOrDefaultAsync(u => u.Id == request.ProjectId, cancellationToken);
            if (project is null)
            {
                throw new NotFoundException($"Project {request.ProjectId} not found");
            }

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

            project.State = ProjectState.DaoRevoked; 
            await projectWriteRepository.UpdateProjectAsync(project, cancellationToken);

            return Result<bool>.Success(true);
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
