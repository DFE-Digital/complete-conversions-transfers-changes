using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateProjectCompletedCommand(
    ProjectId ProjectId
) : IRequest<Result<bool>>;

internal class UpdateProjectCompletedCommandHandler(
    ICompleteRepository<Project> projectRepository)
    : IRequestHandler<UpdateProjectCompletedCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateProjectCompletedCommand request,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.Query()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

        if (project == null)
            return Result<bool>.Success(false);

        project!.CompletedAt = DateTime.UtcNow;
        project!.State = ProjectState.Completed;
        await projectRepository.UpdateAsync(project, cancellationToken);
        return Result<bool>.Success(true);
    }
}