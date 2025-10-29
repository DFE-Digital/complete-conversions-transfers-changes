using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.KeyContacts.Commands;
using Dfe.Complete.Application.KeyContacts.Queries;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmChairOfGovernorsDetailsTask
{
    public class ConfirmChairOfGovernorsDetailsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmChairOfGovernorsDetailsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmChairOfGovernorsDetails)
    {
        [BindProperty]
        public Guid? ChairOfGovernorserContactId { get; set; }

        [BindProperty]
        public Guid? KeyContactId { get; set; }

        public List<ContactDto>? Contacts { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            var contacts = await Sender.Send(new GetContactsForProjectByCategoryQuery(Project.Id, ContactCategory.SchoolOrAcademy));
            var chairOfGovernorsKeyContactDto = await Sender.Send(new GetKeyContactsForProjectQuery(Project.Id));

            ChairOfGovernorserContactId = chairOfGovernorsKeyContactDto?.Value?.ChairOfGovernorsId?.Value;
            KeyContactId = chairOfGovernorsKeyContactDto?.Value?.Id?.Value;

            Contacts = contacts?.Value ?? [];

            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateChairOfGovernorsCommand(new KeyContactId(KeyContactId.GetValueOrDefault()), new ContactId(ChairOfGovernorserContactId.GetValueOrDefault())));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
