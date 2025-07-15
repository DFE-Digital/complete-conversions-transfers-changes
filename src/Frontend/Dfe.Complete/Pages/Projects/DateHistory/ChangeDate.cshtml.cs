using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Constants;
using Dfe.Complete.Pages.Projects.ProjectView;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.DateHistory
{
    public class ChangeDateProjectModel(ISender sender, ErrorService errorService) : ProjectLayoutModel(sender, ConversionDateHistoryNavigation)
    {
        [BindProperty]
        [Required(ErrorMessage = "Enter a valid month and year for the revised date, like 9 2024")]
        [Display(Name = "Significant Date")]
        public DateOnly? SignificantDate { get; set; }
        
        public async Task<IActionResult> OnPost()
        {
            await base.OnGetAsync();

            if (SignificantDate == Project.SignificantDate)
            {
                ModelState.AddModelError(nameof(SignificantDate), "The new date cannot be the same as the current date. Check you have entered the correct date.");
            }
            
            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }
            
            TempData["SignificantDate"] = SignificantDate?.ToString();
            
            return Redirect(FormatRouteWithProjectId(RouteConstants.ChangeProjectDateHistoryReason));
        }
    };
}
