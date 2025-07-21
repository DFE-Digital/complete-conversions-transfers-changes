using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 

namespace Dfe.Complete.Pages.Projects.List.HandoverProjects
{
    [Authorize(policy: UserPolicyConstants.CanViewAllProjectsHandover)]
    public class HandoverProjectCheckModel(ISender sender) : AllProjectsModel(HandoverNavigation)
    {
        [BindProperty(SupportsGet = true, Name = "projectId")]
        public Guid ProjectId { get; set; }

        public ProjectWithEstablishmentQueryModel Project { get; set; } = default!;
        public async Task OnGetAsync()
        {
            var project = await sender.Send(new GetProjectWithEstablishmentByIdQuery(new ProjectId(ProjectId)));
            if(project.IsSuccess is false)
            {
                throw new ApplicationException($"An error occurred when fetching project {ProjectId}");
            }
            Project = project.Value!;
        }
        public  IActionResult OnPost()
        {
            return Redirect(string.Format(RouteConstants.NewHandoverProject, ProjectId));
        }
    }
}
