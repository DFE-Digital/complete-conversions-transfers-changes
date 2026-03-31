using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject;

public record ClearProjectOnHoldCommand(
    [Required] ProjectId ProjectId
) : IRequest<Result<bool>>;

internal class ClearProjectOnHoldCommandHandler(
    ICompleteRepository<Project> projectRepository)
    : IRequestHandler<ClearProjectOnHoldCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ClearProjectOnHoldCommand request,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.Query()
                .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

        if (project == null)
            return Result<bool>.Success(false);

        project!.OnHoldDate = null;
        await projectRepository.UpdateAsync(project, cancellationToken);
        return Result<bool>.Success(true);
    }
}