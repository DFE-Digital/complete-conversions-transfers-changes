using Dfe.Complete.Constants;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LandQuestionnaireTask
{
    public class LandQuestionnaireTaskModel(ISender sender, ILogger<LandQuestionnaireTaskModel> _logger) : BaseProjectPageModel(sender, _logger)
    {
        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }
        
        public async Task<IActionResult> OnPost()
        {
            return Redirect(string.Format(RouteConstants.ProjectLandQuestionnaireTask, ProjectId));
        }
    }
}
