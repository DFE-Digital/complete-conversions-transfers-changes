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

public class ExternalContacts(ISender sender, ILogger<ExternalContacts> logger) :  ProjectLayoutModel(sender, ExternalContactsNavigation)
{
    private readonly ISender _sender = sender;
    
    public List<Contact> EstablishmentContacts = new();
    public List<Contact> IncomingTrustContacts = new();
    public List<Contact> OutgoingTrustContacts = new();
    public List<Contact> LocalAuthorityContacts = new();
    public List<Contact> SolicitorContacts = new();
    public List<Contact> DioceseContacts = new();
    public List<Contact> OtherContacts = new();
    public List<Contact> ParliamentaryContacts = new();
    public override async Task<IActionResult> OnGet()
    {
        await base.OnGet();

        var projectQuery = new GetContactsForProjectQuery(Project.Id);

        var projectContacts = await _sender.Send(projectQuery);

        if (projectContacts is { IsSuccess: true, Value: not null })
        {
            EstablishmentContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.SchoolOrAcademy));
            IncomingTrustContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.IncomingTrust));
            OutgoingTrustContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.OutgoingTrust));
            SolicitorContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.Solicitor));
            DioceseContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.Diocese));
            OtherContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Type == ContactCategory.Other));
        }

        if (Establishment.LocalAuthorityCode == null) return Page();
        var laQuery = new GetLocalAuthorityByCodeQuery(Establishment.LocalAuthorityCode);

        var la = await _sender.Send(laQuery);

        if (la is not { IsSuccess: true, Value: not null }) return Page();
        
        var laContactQuery = new GetContactsForLocalAuthorityQuery(la.Value.Id);

        var laContacts = await _sender.Send(laContactQuery);
            
        if (laContacts is { IsSuccess: true, Value: not null })
        {
            LocalAuthorityContacts.AddRange(laContacts.Value);
        }

        return Page();
    }
}