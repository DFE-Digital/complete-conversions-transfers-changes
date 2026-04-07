using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record UpdateProjectOnHoldCommand(
    [Required] ProjectId ProjectId
) : IRequest<Result<bool>>;

internal class UpdateProjectOnHoldCommandHandler(
    ICompleteRepository<Project> projectRepository)
    : IRequestHandler<UpdateProjectOnHoldCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateProjectOnHoldCommand request,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.Query()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

        if (project == null)
            return Result<bool>.Success(false);

        project!.OnHoldDate = DateOnly.FromDateTime(DateTime.Now);
        await projectRepository.UpdateAsync(project, cancellationToken);
        return Result<bool>.Success(true);
    }
}