using Dfe.Complete.Application.Contacts.Models;
using Dfe.Complete.Application.Contacts.Queries;
using Dfe.Complete.Application.LocalAuthorities.Queries.GetLocalAuthority;
using Dfe.Complete.Application.Services.PersonsApi;
using Dfe.Complete.Domain.Constants;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using Dfe.Complete.Models.ExternalContact;
using Dfe.Complete.Pages.Projects.ProjectView;
using DocumentFormat.OpenXml.Wordprocessing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace Dfe.Complete.Pages.Projects.ExternalContacts;

[Authorize(Policy = UserPolicyConstants.CanViewEditDeleteContact)]
public class ExternalContacts(ISender sender, ILogger<ExternalContacts> logger, IDistributedCache cache)
    : ProjectLayoutModel(sender, logger, ExternalContactsNavigation)
{
    private readonly DistributedCacheEntryOptions DefaultCacheOptions = new DistributedCacheEntryOptions()
                 .SetAbsoluteExpiration(TimeSpan.FromMinutes(20));                 

    private readonly IDistributedCache _cache = cache;

    public List<ExternalContactModel> EstablishmentContacts { get; set; } = [];
    public List<ExternalContactModel> IncomingTrustContacts { get; set; } = [];
    public List<ExternalContactModel> OutgoingTrustContacts { get; set; } = [];
    public List<ExternalContactModel> LocalAuthorityContacts { get; set; } = [];
    public List<ExternalContactModel> SolicitorContacts { get; set; } = [];
    public List<ExternalContactModel> DioceseContacts { get; set; } = [];
    public List<ExternalContactModel> OtherContacts { get; set; } = [];
    public ConstituencyMemberContactDto? ParliamentaryContact { get; set; } = null;
    public string LocalAuthorityName { get; set; } = "";

    [BindProperty(Name = $"new_transfer_contact_form[contact_type]")]
    public string? TransferContactType { get; set; }
    
    [BindProperty(Name = $"new_conversion_contact_form[contact_type]")]
    public string? ConversionContactType { get; set; }
    

    public override async Task<IActionResult> OnGetAsync()
    {
        await base.OnGetAsync();

        var projectQuery = new GetContactsForProjectQuery(Project.Id);

        var projectContacts = await Sender.Send(projectQuery);

        if (projectContacts is { IsSuccess: true, Value: not null })
        {
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
            LocalAuthorityContacts.AddRange(
                projectContacts.Value.FindAll(contact => contact.Category == ContactCategory.LocalAuthority).Select(
                    contact =>
                        new ExternalContactModel(contact, 
                            true, 
                            contact.Id == Project.MainContactId,
                            contact.Id == Project.LocalAuthorityMainContactId)));
            SolicitorContacts.AddRange(
                projectContacts.Value.FindAll(contact => contact.Category == ContactCategory.Solicitor).Select(
                    contact =>
                        new ExternalContactModel(contact, true)));
            DioceseContacts.AddRange(projectContacts.Value
                .FindAll(contact => contact.Category == ContactCategory.Diocese)
                .Select(contact =>
                    new ExternalContactModel(contact, true)));
            OtherContacts.AddRange(projectContacts.Value.FindAll(contact => contact.Category == ContactCategory.Other)
                .Select(contact =>
                    new ExternalContactModel(contact,
                        true, 
                        contact.Id == Project.MainContactId,
                        ShowOrganisation: true)));
        }

        if (Establishment.LocalAuthorityCode == null) return Page();
        var laQuery = new GetLocalAuthorityByCodeQuery(Establishment.LocalAuthorityCode);

        var la = await Sender.Send(laQuery);

        if (la is not { IsSuccess: true, Value: not null }) return Page();

        LocalAuthorityName = la.Value.Name;

        var laContactQuery = new GetContactsForLocalAuthorityQuery(la.Value.Id);

        var laContacts = await Sender.Send(laContactQuery);

        if (laContacts is { IsSuccess: true, Value: not null })
        {
            LocalAuthorityContacts.AddRange(laContacts.Value.Select(contact =>
                new ExternalContactModel(contact, false)));
        }       

        if (Establishment.ParliamentaryConstituency != null && !string.IsNullOrWhiteSpace(Establishment.ParliamentaryConstituency.Name))
        {
            var cacheKey = $"GetContactByConstituency-{Establishment.ParliamentaryConstituency.Name}";
            ConstituencyMemberContactDto? constituencyMember = await _cache.GetOrSetAsync<ConstituencyMemberContactDto>(
                cacheKey,
                async () =>
                {
                    var result = await GetContactByConstituency(Establishment.ParliamentaryConstituency.Name);
                    return result;
                },
                DefaultCacheOptions
            );

            if (constituencyMember != null)
            {
                this.ParliamentaryContact = constituencyMember;
            }
        }
        return Page();
    }   

    private async Task<ConstituencyMemberContactDto?> GetContactByConstituency(string constituencyName)
    {
        var getContactyByConstituency = new GetContactByConstituency(constituencyName);
        var result = await Sender.Send(getContactyByConstituency);
        return result?.Value ?? null;
    }
   
}