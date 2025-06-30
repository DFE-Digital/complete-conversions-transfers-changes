using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Users.Queries.GetUser;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dfe.Complete.Pages.Projects.ExternalContacts;

public class ExternalContacts(ISender sender, ILogger<ExternalContacts> logger)
    : ProjectLayoutModel(sender, ExternalContactsNavigation)
{
    private readonly ISender _sender = sender;

    public List<ExternalContactModel> EstablishmentContacts = [];
    public List<ExternalContactModel> IncomingTrustContacts = [];
    public List<ExternalContactModel> OutgoingTrustContacts = [];
    public List<ExternalContactModel> LocalAuthorityContacts = [];
    public List<ExternalContactModel> SolicitorContacts = [];
    public List<ExternalContactModel> DioceseContacts = [];
    public List<ExternalContactModel> OtherContacts = [];
    public List<ExternalContactModel> ParliamentaryContacts = [];
    public bool ProjectContactsExist;

    public override async Task<IActionResult> OnGet()
    {
        await base.OnGet();

        var projectQuery = new GetContactsForProjectQuery(Project.Id);

        var projectContacts = await _sender.Send(projectQuery);

        if (projectContacts is { IsSuccess: true, Value: not null })
        {
            ProjectContactsExist = projectContacts.Value.Count > 0;
            EstablishmentContacts.AddRange(projectContacts.Value
                .FindAll(contact => contact.Type == ContactCategory.SchoolOrAcademy).Select(contact =>
                    new ExternalContactModel(contact,
                        true,
                        contact.Id == Project.MainContactId,
                        contact.Id == Project.EstablishmentMainContactId
                    )));
            IncomingTrustContacts.AddRange(
                projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.IncomingTrust).Select(
                    contact =>
                        new ExternalContactModel(contact,
                            true,
                            contact.Id == Project.MainContactId,
                            contact.Id == Project.IncomingTrustMainContactId
                        )));
            OutgoingTrustContacts.AddRange(
                projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.OutgoingTrust).Select(
                    contact =>
                        new ExternalContactModel(contact,
                            true,
                            contact.Id == Project.MainContactId,
                            contact.Id == Project.OutgoingTrustMainContactId
                        )));
            SolicitorContacts.AddRange(
                projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.Solicitor).Select(contact =>
                    new ExternalContactModel(contact, true)));
            DioceseContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.Diocese)
                .Select(contact =>
                    new ExternalContactModel(contact, true)));
            OtherContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.Other)
                .Select(contact =>
                    new ExternalContactModel(contact, true)));
        }

        if (Establishment.LocalAuthorityCode == null) return Page();
        var laQuery = new GetLocalAuthorityByCodeQuery(Establishment.LocalAuthorityCode);

        var la = await _sender.Send(laQuery);

        if (la is not { IsSuccess: true, Value: not null }) return Page();

        var laContactQuery = new GetContactsForLocalAuthorityQuery(la.Value.Id);

        var laContacts = await _sender.Send(laContactQuery);

        if (laContacts is { IsSuccess: true, Value: not null })
        {
            LocalAuthorityContacts.AddRange(laContacts.Value.Select(contact =>
                new ExternalContactModel(contact, false)));
        }

        // if (Establishment.ParliamentaryConstituency?.Code is null) return Page();
        //
        // var mpContactQuery = new GetContactForConstituencyQuery(Establishment.ParliamentaryConstituency.Code);
        //
        // var mpContact = await _sender.Send(mpContactQuery);
        //
        // if (mpContact is { IsSuccess: true, Value: not null })
        // {
        //     var mp = new ExternalContactModel(mpContact.Value, false);
        //     ParliamentaryContacts.Add(mp);
        // }


        return Page();
    }
}