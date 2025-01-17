using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Validators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.MatConversion;

public class CreateNewProject : PageModel
{

    [BindProperty]
    [Required]
    // [Urn]
    [Display(Name = "Urn")]
    public string URN { get; set; }
    
    [BindProperty]
    [Ukprn]
    [Required]
    [Display(Name = "Trust reference number (TRN)")]
    public string TrustReferenceNumber { get; set; }
    
    [BindProperty]
    [Display(Name = "Trust name")]
    [Required]
    public string TrustName { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Enter a date for the Advisory Board Date, like 1 4 2023")]
    [Display(Name = "Advisory Board Date")]
    public DateTime? AdvisoryBoardDate { get; set; }
    
    
    [BindProperty] 
    public string AdvisoryBoardConditions { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Enter a date for the Provisional Conversion Date, like 1 4 2023")]
    [Display(Name = "Provisional Conversion Date")]
    public DateTime? ProvisionalConversionDate { get; set; }


    [BindProperty]
    [SharePointLink]
    [Required]
    [Display(Name = "School or academy SharePoint link")]
    public string SchoolSharePointLink { get; set; }

    [BindProperty]
    [SharePointLink]
    [Required]
    [Display(Name = "Incoming trust SharePoint link")]
    public string IncomingTrustSharePointLink { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "State if this project will be handed over to the Regional casework services team. Choose yes or no")]
    [Display(Name = "Is Handing To RCS")]
    public bool? IsHandingToRCS { get; set; }
    
    [BindProperty] 
    public string HandoverComments { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Select directive academy order or academy order, whichever has been used for this conversion")]
    [Display(Name = "Directive Academy Order")]
    public bool? DirectiveAcademyOrder { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "State if the conversion is due to 2RI. Choose yes or no")]
    [Display(Name = "IsDueTo2RI")]
    public bool? IsDueTo2RI { get; set; } 
    
    public void OnGet()
    {
        
    }
}