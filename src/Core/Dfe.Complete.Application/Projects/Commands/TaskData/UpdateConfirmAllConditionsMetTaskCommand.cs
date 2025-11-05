using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmAllConditionsMetTaskCommand(ProjectId ProjectId,
        bool? Confirm) : IRequest<Result<bool>>;

    public class UpdateConfirmAllConditionsMetTaskCommandHandler(
        IProjectReadRepository projectReadRepository,
        IProjectWriteRepository projectWriteRepository)
        : IRequestHandler<UpdateConfirmAllConditionsMetTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmAllConditionsMetTaskCommand request, CancellationToken cancellationToken)
        {
            var project = await projectReadRepository.Projects.FirstOrDefaultAsync(p => p.Id == request.ProjectId, cancellationToken)
              ?? throw new NotFoundException($"Project with {request.ProjectId} id not found.");

            project.AllConditionsMet = request.Confirm;
            project.UpdatedAt = DateTime.UtcNow;
            await projectWriteRepository.UpdateProjectAsync(project, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}