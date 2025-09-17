using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects
{
    public class CompleteProjectModel(ISender sender, ILogger<CompleteProjectModel> logger)
    : BaseProjectPageModel(sender, logger)   
    {
        public void OnGet()
        {
        }

        public void OnPost()
        {

        }
    }
}
