using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmChildrenCentreTask;

public class ConfirmChildrenCentreTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmChildrenCentreTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmChildrenCentre, projectPermissionService)
{
    [BindProperty(Name = "not_applicable")]
    public bool? NotApplicable { get; set; }

    [BindProperty(Name = "children_centre_local_authority")]
    public bool? ChildrenCentreLocalAuthority { get; set; }

    [BindProperty(Name = "children_centre_academy_trust")]
    public bool? ChildrenCentreAcademyTrust { get; set; }

    [BindProperty(Name = "children_centre_staffing_and_transfer_reviewed")]
    public bool? ChildrenCentreStaffingAndTransferReviewed { get; set; }

    [BindProperty(Name = "children_centre_land_lease_shared_agreed")]
    public bool? ChildrenCentreLandLeaseSharedAgreed { get; set; }

    [BindProperty(Name = "children_centre_funding_pension_reviewed")]
    public bool? ChildrenCentreFundingPensionReviewed { get; set; }

    [BindProperty(Name = "children_centre_legal_and_governance_reviewed")]
    public bool? ChildrenCentreLegalAndGovernanceReviewed { get; set; }
    
    [BindProperty]
    public Guid? TasksDataId { get; set; }

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();

        TasksDataId = Project.TasksDataId?.Value;

        if (InvalidTaskRequestByProjectType())
            return Redirect(RouteConstants.ErrorPage);
        
        NotApplicable = ConversionTaskData.ChildrenCentreNotApplicable;
        ChildrenCentreLocalAuthority = ConversionTaskData.ChildrenCentreLocalAuthority;
        ChildrenCentreAcademyTrust = ConversionTaskData.ChildrenCentreAcademyTrust;
        ChildrenCentreStaffingAndTransferReviewed = ConversionTaskData.ChildrenCentreStaffingAndTransferReviewed;
        ChildrenCentreLandLeaseSharedAgreed = ConversionTaskData.ChildrenCentreLandLeaseSharedAgreed;
        ChildrenCentreFundingPensionReviewed = ConversionTaskData.ChildrenCentreFundingPensionReviewed;
        ChildrenCentreLegalAndGovernanceReviewed = ConversionTaskData.ChildrenCentreLegalAndGovernanceReviewed;

        return Page();
    }
    
    public async Task<IActionResult> OnPost()
    {
        if (NotApplicable == true)
        {
            ChildrenCentreAcademyTrust = false;
            ChildrenCentreFundingPensionReviewed = false;
            ChildrenCentreLandLeaseSharedAgreed = false;
            ChildrenCentreLegalAndGovernanceReviewed = false;
            ChildrenCentreLocalAuthority = false;
            ChildrenCentreStaffingAndTransferReviewed = false;
        }
        
        await Sender.Send(new UpdateChildrenCentreTaskCommand(
            new TaskDataId(TasksDataId.GetValueOrDefault()), 
            NotApplicable,
            ChildrenCentreLocalAuthority,
            ChildrenCentreAcademyTrust,
            ChildrenCentreStaffingAndTransferReviewed,
            ChildrenCentreLandLeaseSharedAgreed,
            ChildrenCentreFundingPensionReviewed,
            ChildrenCentreLegalAndGovernanceReviewed
        ));
        SetTaskSuccessNotification();
        return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
    }
}