using System.ComponentModel.DataAnnotations;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Projects.Commands.UpdateProject;
using Dfe.Complete.Application.Projects.Queries.GetEstablishment;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Dfe.Complete.Validators;
using Microsoft.AspNetCore.Authorization;

namespace Dfe.Complete.Pages.Projects.ServiceSupport.ConversionURNs
{
    [Authorize(policy: UserPolicyConstants.CanViewServiceSupport)]
    public class CreateAcademyUrnModel(ISender sender, 
        ErrorService errorService, ILogger<CreateAcademyUrnModel> logger) : BaseProjectPageModel(sender, logger)
    {
        [BindProperty]
        [AcademyUrn]
        [Display(Name = "Urn")]
        public string URN { get; set; }
        
        public EstablishmentDto? SelectedAcademy { get; set; }

        public async Task<IActionResult> OnPost(CancellationToken cancellationToken)
        {
            await OnGetAsync();

            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }
            
            var result = await Sender.Send(new GetEstablishmentByUrnQuery(URN.ToInt()), cancellationToken);
            SelectedAcademy = result.Value;

            return Page();
        }
        
        public async Task<IActionResult> OnPostSave(CancellationToken cancellationToken)
        {
            await OnGetAsync();

            if (!ModelState.IsValid)
            {
                errorService.AddErrors(ModelState);
                return Page();
            }
            
            var projectId = new ProjectId(Guid.Parse(ProjectId));
            var urn = new Urn(URN.ToInt());

            // Update the project
            var updateAcademyUrnCommand = new UpdateAcademyUrnCommand(projectId, urn);
            await Sender.Send(updateAcademyUrnCommand, cancellationToken);
            
            var successMessage = string.Format("Academy URN {0} added to {1}, {2}", URN, Establishment?.Name, Establishment?.Urn);

            TempData.SetNotification(
                NotificationType.Success,
                "Success",
                successMessage);

            return Redirect(RouteConstants.ServiceSupportProjectsWithoutAcademyUrn);
        }
        
        public string ToDisplayAddress(AddressDto addressDto)
        {
            var lines = new List<string>
            {
                addressDto.Street
            };

            if (!string.IsNullOrWhiteSpace(addressDto.Additional))
            {
                lines.Add(addressDto.Additional);
            }

            lines.Add(addressDto.Town);
            lines.Add(addressDto.County);
            lines.Add(addressDto.Postcode);

            return string.Join("<br>", lines);
        }
    }
}
