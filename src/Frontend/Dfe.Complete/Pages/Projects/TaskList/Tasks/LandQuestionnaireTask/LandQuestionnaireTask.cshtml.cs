using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.LandQuestionnaireTask
{
    public class LandQuestionnaireTaskModel(ISender sender, IAuthorizationService authorizationService) : BaseProjectTaskModel(sender, authorizationService,"")
    {
        [BindProperty(Name = "cleared")]
        public bool? Cleared { get; set; }

        [BindProperty(Name = "received")]
        public bool? Received { get; set; }

        [BindProperty(Name = "signed")]
        public bool? Signed { get; set; }

        [BindProperty(Name = "saved")]
        public bool? Saved { get; set; }
    }
}
