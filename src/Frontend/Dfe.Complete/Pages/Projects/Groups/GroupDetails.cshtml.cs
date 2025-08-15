using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.Groups
{
    public class ProjectGroupDetailsModel(ISender sender) : BaseProjectsPageModel(String.Empty)
    {
        [BindProperty(SupportsGet = true, Name = "groupId")]
        public string GroupId { get; set; }
        public ProjectGroupDetails ProjectGroupDetails { get; set; }
        
        public async Task OnGet()
        {
            var request = new GetProjectGroupDetailsQuery(new ProjectGroupId(Guid.Parse(GroupId)));
            var response = await sender.Send(request);
            
            ProjectGroupDetails = response.Value;
        }
    }
}
