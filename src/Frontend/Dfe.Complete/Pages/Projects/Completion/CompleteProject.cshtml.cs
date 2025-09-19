using Dfe.Complete.Constants;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects
{
    public class CompleteProjectModel(ISender sender, ILogger<CompleteProjectModel> logger)
    : BaseProjectPageModel(sender, logger)   
    {  
        public async override Task<IActionResult> OnGetAsync()
        {
            await UpdateCurrentProject();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await UpdateCurrentProject();

            var canComplete = false;

            if(canComplete)
            {
                return await OnGetAsync();
            }

            else
            {
                TempData.SetNotification(
                      NotificationType.Success,
                      "Success",
                      "Your note has been edited"
                );

                return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));               
            }           
        }
    }
}
