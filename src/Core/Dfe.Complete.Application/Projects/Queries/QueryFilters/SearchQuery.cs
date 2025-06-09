using System.Text.RegularExpressions;
using Dfe.Complete.Application.Common.Queries;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.QueryFilters;

public class SearchQuery(string? t) : IQueryObject<Project>
{
    private readonly string? _term = t?.Trim();

    public IQueryable<Project> Apply(IQueryable<Project> q)
    {
        var timeSpan = TimeSpan.FromMilliseconds(100);
        if (string.IsNullOrEmpty(_term)) return q;

        if (Regex.IsMatch(_term, @"^\d{6}$", RegexOptions.None, timeSpan))     // URN
            return q.Where(p => p.Urn == new Urn(int.Parse(_term)));

        if (Regex.IsMatch(_term, @"^\d{8}$", RegexOptions.None, timeSpan))     // UKPRN
        {
            var vo = new Ukprn(int.Parse(_term));
            return q.Where(p => p.IncomingTrustUkprn == vo || p.OutgoingTrustUkprn == vo);
        }

        if (Regex.IsMatch(_term, @"^\d{4}$", RegexOptions.None, timeSpan))     // Establishment Number
            return q.Where(p => p.GiasEstablishment.EstablishmentNumber == _term);

        return q.Where(p =>
            EF.Functions.Like(p.GiasEstablishment.Name, $"%{_term}%"));
    }
}