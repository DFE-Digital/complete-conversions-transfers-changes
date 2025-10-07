using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.KeyContacts.Commands;
using Dfe.Complete.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.IncomingTrustCeoTask
{
    public class IncomingTrustCeoTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<IncomingTrustCeoTaskModel> logger)
        : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmIncomingTrustCEOContact)
    {
        [BindProperty]
        public Guid? IncomingTrustCeoContactId { get; set; }      

        [BindProperty]
        public Guid? KeyContactId { get; set; }

        [BindProperty]
        public ProjectType? Type { get; set; }

        public List<ContactDto>? Contacts { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            var contacts = await Sender.Send(new GetContactsForProjectByCategoryQuery(Project.Id, ContactCategory.IncomingTrust));
            var incomingTrustCeoKeyContactDto = await Sender.Send(new GetKeyContactsForProjectQuery(Project.Id));

            IncomingTrustCeoContactId = incomingTrustCeoKeyContactDto?.Value?.IncomingTrustCeoId?.Value;
            KeyContactId = incomingTrustCeoKeyContactDto?.Value?.Id?.Value;

            Contacts = contacts?.Value ?? [];
            Type = Project.Type;
                       
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateIncomingTrustCeoCommand(new KeyContactId(KeyContactId.GetValueOrDefault()), new ContactId(IncomingTrustCeoContactId.GetValueOrDefault())));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
