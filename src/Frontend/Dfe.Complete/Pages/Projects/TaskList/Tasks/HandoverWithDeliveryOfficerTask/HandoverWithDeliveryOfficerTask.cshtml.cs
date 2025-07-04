using Dfe.Complete.Application.Projects.Commands.ConversionTasks;
using Dfe.Complete.Application.Projects.Queries.ConversionTasks;
using Dfe.Complete.Application.Projects.Commands.TransferTasks;
using Dfe.Complete.Application.Projects.Queries.TransferTasks;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Pages.Projects;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.HandoverWithDeliveryOfficerTask;

using Microsoft.AspNetCore.Authorization;

public class HandoverWithDeliveryOfficerTaskModel(ISender sender, IAuthorizationService authorizationService) : ProjectTaskModel(sender, authorizationService)
{
    [BindProperty(Name = "not-applicable")]
    public bool NotApplicable { get; set; }

    [BindProperty(Name = "review-project-information")]
    public bool ReviewProjectInformation { get; set; }

    [BindProperty(Name = "make-notes")]
    public bool MakeNotes { get; set; }

    [BindProperty(Name = "attend-handover-meeting")]
    public bool AttendHandoverMeeting { get; set; }


    public async override Task<IActionResult> OnGetAsync()
    {
        TaskIdentifier = "handover";
        Title = "Handover with regional delivery officer";
        await base.OnGetAsync();

        if (Project.Type == ProjectType.Conversion)
        {
            var response = await sender.Send(new GetConversionHandoverTaskDataByProjectIdQuery(new ProjectId(new Guid(ProjectId))));
            if (!response.IsSuccess || response.Value == null)
                throw new InvalidOperationException($"Failed to retrieve handover task data: {response.Error}");

            var taskData = response.Value;
            NotApplicable = taskData.NotApplicable ?? false;
            ReviewProjectInformation = taskData.ReviewProjectInformation ?? false;
            MakeNotes = taskData.MakeNotes ?? false;
            AttendHandoverMeeting = taskData.AttendHandoverMeeting ?? false;
        }
        else if (Project.Type == ProjectType.Transfer)
        {
            var response = await sender.Send(new GetTransferHandoverTaskDataByProjectIdQuery(new ProjectId(new Guid(ProjectId))));
            if (!response.IsSuccess || response.Value == null)
                throw new InvalidOperationException($"Failed to retrieve handover task data: {response.Error}");

            var taskData = response.Value;
            NotApplicable = taskData.NotApplicable ?? false;
            ReviewProjectInformation = taskData.ReviewProjectInformation ?? false;
            MakeNotes = taskData.MakeNotes ?? false;
            AttendHandoverMeeting = taskData.AttendHandoverMeeting ?? false;
        }
        else
        {
            throw new InvalidOperationException($"Unsupported project type: {Project.Type}");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        TaskIdentifier = "handover";
        await base.OnGetAsync();
        if (Project.Type == ProjectType.Conversion)
        {
            await sender.Send(new UpdateConversionHandoverTaskDataByProjectIdCommand(
                new ProjectId(new Guid(ProjectId)),
                NotApplicable,
                ReviewProjectInformation,
                MakeNotes,
                AttendHandoverMeeting
            ));
        }
        else if (Project.Type == ProjectType.Transfer)
        {
            await sender.Send(new UpdateTransferHandoverTaskDataByProjectIdCommand(
                new ProjectId(new Guid(ProjectId)),
                NotApplicable,
                ReviewProjectInformation,
                MakeNotes,
                AttendHandoverMeeting
            ));
        }
        else
        {
            throw new InvalidOperationException($"Unsupported project type: {Project.Type}");
        }

        return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
    }
}
