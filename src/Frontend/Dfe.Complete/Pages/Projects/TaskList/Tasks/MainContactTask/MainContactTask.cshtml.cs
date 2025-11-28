using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.Projects.Commands.TaskData;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.MainContactTask
{
    public class MainContactTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<MainContactTaskModel> logger, IProjectPermissionService projectPermissionService)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.MainContact, projectPermissionService)
    {
        [BindProperty]
        public Guid? MainContactId { get; set; }
        public List<ContactDto>? Contacts { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            var contacts = await Sender.Send(new GetContactsForProjectAndLocalAuthorityQuery(Project.Id,
                Project.LocalAuthorityId));

            Contacts = contacts?.Value ?? [];
            MainContactId = Project.MainContactId?.Value;
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateMainContactTaskCommand(new ProjectId(Guid.Parse(ProjectId)), new ContactId(MainContactId!.Value)));
            SetTaskSuccessNotification();

            return LocalRedirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
