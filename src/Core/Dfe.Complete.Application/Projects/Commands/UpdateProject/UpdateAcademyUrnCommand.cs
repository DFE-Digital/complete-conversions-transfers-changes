using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.UpdateProject
{
    public record UpdateAcademyUrnCommand(ProjectId ProjectId, Urn Urn) : IRequest;

    public class UpdateAcademyUrnCommandHandler(
        ICompleteRepository<Project> projectRepository)
        : IRequestHandler<UpdateAcademyUrnCommand>
    {
        public async Task Handle(UpdateAcademyUrnCommand request, CancellationToken cancellationToken)
        {
            var project = await projectRepository.Query().FirstOrDefaultAsync(x => x.Id == request.ProjectId, cancellationToken);

            if (project == null)
            {
                return;
            }

            project.AddAcademyUrn(request.Urn);
            await projectRepository.UpdateAsync(project, cancellationToken);
        }
    }
}