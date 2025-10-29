using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    public record UpdateDeleteProjectCommand([Required] ProjectId ProjectId) : IRequest<Result<bool>>;
    internal class UpdateDeleteProjectCommandHandler(
    IProjectReadRepository projectReadRepository,
    IProjectWriteRepository projectWriteRepository)
    : IRequestHandler<UpdateDeleteProjectCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateDeleteProjectCommand request,
            CancellationToken cancellationToken)
        {
            var project = await projectReadRepository.ProjectsNoIncludes
                    .FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken) 
                    ?? throw new NotFoundException($"Project with {request.ProjectId} is not found.", "ProjectId");
              
            project!.UpdatedAt = DateTime.UtcNow;
            project!.State = ProjectState.Deleted;

            await projectWriteRepository.UpdateProjectAsync(project, cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}
