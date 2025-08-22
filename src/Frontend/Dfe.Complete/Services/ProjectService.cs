using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services.Interfaces;
using MediatR;

namespace Dfe.Complete.Services
{
    public class ProjectService(ISender sender, ILogger<ProjectService> logger) : IProjectService
    {
        protected readonly ISender Sender = sender;
        private readonly ILogger<ProjectService> Logger  = logger;
      
        public async Task<ProjectDto?> GetProjectById(string projectId)
        {
            ProjectDto? project = null;
            var success = Guid.TryParse(projectId, out var guid);

            if (!success)
            {
                Logger.LogWarning("{ProjectId} is not a valid Guid.", projectId);
                return project;
            }

            var query = new GetProjectByIdQuery(new ProjectId(guid));
            var result = await Sender.Send(query);

            if (!result.IsSuccess || result.Value == null)
            {
                Logger.LogWarning("Project {ProjectId} does not exist.", projectId);
                return project;
            }
            else
            {   
                project = result.Value;
            }

            return project;
        }
       
    }
}
