using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateMainContactTaskCommand(
        ProjectId ProjectId,
        ContactId MainContactId
    ) : IRequest<Result<bool>>;

    internal class UpdateMainContactTaskCommandHandler(
        IProjectReadRepository projectReadRepository, 
        IProjectWriteRepository projectWriteRepository)
        : IRequestHandler<UpdateMainContactTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateMainContactTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await projectReadRepository.Projects
                .FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
                ?? throw new NotFoundException($"Project {request.ProjectId.Value} not found.");
            var now = DateTime.UtcNow;
            project.MainContactId = request.MainContactId;
            project.UpdatedAt = now;
            await projectWriteRepository.UpdateProjectAsync(project, cancellationToken);
            
            return Result<bool>.Success(true);
        }
    }
}
