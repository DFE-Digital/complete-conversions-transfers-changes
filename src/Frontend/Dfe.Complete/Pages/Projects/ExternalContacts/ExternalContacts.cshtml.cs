using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Pages.Projects.ProjectView;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dfe.Complete.Pages.Projects.ExternalContacts;

public class ExternalContacts(ISender sender)
    : ProjectLayoutModel(sender, ExternalContactsNavigation)
{
    private readonly ISender _sender = sender;

    public List<ExternalContactModel> EstablishmentContacts { get; set; } = [];
    public List<ExternalContactModel> IncomingTrustContacts { get; set; } = [];
    public List<ExternalContactModel> OutgoingTrustContacts { get; set; } = [];
    public List<ExternalContactModel> LocalAuthorityContacts { get; set; }= [];
    public List<ExternalContactModel> SolicitorContacts { get; set; } = [];
    public List<ExternalContactModel> DioceseContacts { get; set; } = [];
    public List<ExternalContactModel> OtherContacts { get; set; } = [];
    public List<ExternalContactModel> ParliamentaryContacts { get; set; } = [];
    public string LocalAuthorityName { get; set; } = "";
    public bool ProjectContactsExist { get; set; }

    public override async Task<IActionResult> OnGet()
    {
        await base.OnGet();

        var projectQuery = new GetContactsForProjectQuery(Project.Id);

        var projectContacts = await _sender.Send(projectQuery);

        if (projectContacts is { IsSuccess: true, Value: not null })
        {
            ProjectContactsExist = projectContacts.Value.Count > 0;
            EstablishmentContacts.AddRange(projectContacts.Value
                .FindAll(contact => contact.Category == ContactCategory.SchoolOrAcademy).Select(contact =>
                    new ExternalContactModel(contact,
                        true,
                        contact.Id == Project.MainContactId,
                        contact.Id == Project.EstablishmentMainContactId
                    )));
            IncomingTrustContacts.AddRange(
                projectContacts.Value.FindAll(contact => contact.Category == ContactCategory.IncomingTrust).Select(
                    contact =>
                        new ExternalContactModel(contact,
                            true,
                            contact.Id == Project.MainContactId,
                            contact.Id == Project.IncomingTrustMainContactId
                        )));
            OutgoingTrustContacts.AddRange(
                projectContacts.Value.FindAll(contact => contact.Category == ContactCategory.OutgoingTrust).Select(
                    contact =>
                        new ExternalContactModel(contact,
                            true,
                            contact.Id == Project.MainContactId,
                            contact.Id == Project.OutgoingTrustMainContactId
                        )));
            SolicitorContacts.AddRange(
                projectContacts.Value.FindAll(contact => contact.Category == ContactCategory.Solicitor).Select(contact =>
                    new ExternalContactModel(contact, true)));
            DioceseContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Category == ContactCategory.Diocese)
                .Select(contact =>
                    new ExternalContactModel(contact, true)));
            OtherContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Category == ContactCategory.Other)
                .Select(contact =>
                    new ExternalContactModel(contact, true)));
        }

        if (Establishment.LocalAuthorityCode == null) return Page();
        var laQuery = new GetLocalAuthorityByCodeQuery(Establishment.LocalAuthorityCode);

        var la = await _sender.Send(laQuery);

        if (la is not { IsSuccess: true, Value: not null }) return Page();

        LocalAuthorityName = la.Value.Name;

        var laContactQuery = new GetContactsForLocalAuthorityQuery(la.Value.Id);

        var laContacts = await _sender.Send(laContactQuery);

        if (laContacts is { IsSuccess: true, Value: not null })
        {
            LocalAuthorityContacts.AddRange(laContacts.Value.Select(contact =>
                new ExternalContactModel(contact, false)));
        }

        if (Establishment.ParliamentaryConstituency?.Name is null) return Page();
        
        var mpContactQuery = new GetContactForConstituencyQuery(Establishment.ParliamentaryConstituency.Name);
        
        var mpContact = await _sender.Send(mpContactQuery);
        
        if (mpContact is { IsSuccess: true, Value: not null })
        {
            var mp = new ExternalContactModel(mpContact.Value, false);
            ParliamentaryContacts.Add(mp);
        }

        return Page();
    }
}