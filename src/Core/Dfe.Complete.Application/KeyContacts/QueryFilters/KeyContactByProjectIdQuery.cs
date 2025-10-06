using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.KeyContacts.Queries.QueryFilters;

// filtering by project ID
public class KeyContactByProjectIdQuery(ProjectId? projectId) : IQueryObject<KeyContact>
{
    public IQueryable<KeyContact> Apply(IQueryable<KeyContact> query)
        => projectId == null ? query : query.Where(keycontact => keycontact.ProjectId == projectId);
}
