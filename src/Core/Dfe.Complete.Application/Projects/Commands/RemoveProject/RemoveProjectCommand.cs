using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Hosting;

namespace Dfe.Complete.Application.Projects.Commands.RemoveProject
{
    public record RemoveProjectCommand(
       Urn Urn) : IRequest<ProjectId>;

    public class RemoveProjectCommandHandler(IHostEnvironment hostEnvironment, ICompleteRepository<Project> projectRepository, ICompleteRepository<ConversionTasksData> conversionTaskRepository)
        : IRequestHandler<RemoveProjectCommand, ProjectId>
    {
        public async Task<ProjectId> Handle(RemoveProjectCommand request, CancellationToken cancellationToken)
        {
            if (!hostEnvironment.IsStaging() && !hostEnvironment.IsDevelopment())
            {
                return false;
            }

            projectRepository.Remove(projectRepository.Get(request.Urn));
        }
    }
}
