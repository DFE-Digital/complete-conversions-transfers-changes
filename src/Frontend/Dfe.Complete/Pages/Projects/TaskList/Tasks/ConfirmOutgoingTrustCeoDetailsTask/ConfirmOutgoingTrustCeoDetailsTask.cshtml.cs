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

namespace Dfe.Complete.Pages.Projects.TaskList.Tasks.ConfirmOutgoingTrustCeoDetails
{
    public class ConfirmOutgoingTrustCeoDetailsTaskModel(ISender sender, IAuthorizationService authorizationService, ILogger<ConfirmOutgoingTrustCeoDetailsTaskModel> logger)
    : BaseProjectTaskModel(sender, authorizationService, logger, NoteTaskIdentifier.ConfirmOutgoingTrustCeoDetails)
    {
        [BindProperty]
        public Guid? OutgoingTrustCeoContactId { get; set; }      

        [BindProperty]
        public Guid? KeyContactId { get; set; }        

        public List<ContactDto>? Contacts { get; set; }

        public override async Task<IActionResult> OnGetAsync()
        {
            await base.OnGetAsync();

            if (InvalidTaskRequestByProjectType())
                return Redirect(RouteConstants.ErrorPage);

            var contacts = await Sender.Send(new GetContactsForProjectByCategoryQuery(Project.Id, ContactCategory.OutgoingTrust));
            var outgoingTrustCeoKeyContactDto = await Sender.Send(new GetKeyContactsForProjectQuery(Project.Id));

            OutgoingTrustCeoContactId = outgoingTrustCeoKeyContactDto?.Value?.OutgoingTrustCeoId?.Value;
            KeyContactId = outgoingTrustCeoKeyContactDto?.Value?.Id?.Value;

            Contacts = contacts?.Value ?? [];
                                   
            return Page();
        }
        public async Task<IActionResult> OnPost()
        {
            await Sender.Send(new UpdateOutgoingTrustCeoCommand(new KeyContactId(KeyContactId.GetValueOrDefault()), new ContactId(OutgoingTrustCeoContactId.GetValueOrDefault())));
            SetTaskSuccessNotification();
            return Redirect(string.Format(RouteConstants.ProjectTaskList, ProjectId));
        }
    }
}
