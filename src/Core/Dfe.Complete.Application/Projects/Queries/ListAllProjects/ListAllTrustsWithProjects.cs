using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllTrustsWithProjectsQuery() : PaginatedRequest<PaginatedResult<List<ListTrustsWithProjectsResultModel>>>;

    public class ListAllTrustsWithProjectsQueryHandler(
        IProjectReadRepository repo, ITrustsV4Client trustsClient, ILogger<ListAllTrustsWithProjectsQueryHandler> logger)
        : IRequestHandler<ListAllTrustsWithProjectsQuery, PaginatedResult<List<ListTrustsWithProjectsResultModel>>>
    {
        public async Task<PaginatedResult<List<ListTrustsWithProjectsResultModel>>> Handle(ListAllTrustsWithProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var baseQ = new StateQuery(ProjectState.Active)
                    .Apply(repo.Projects.AsNoTracking());

                var nonMatGroups = await new FormAMatQuery(false)
                    .Apply(baseQ)
                    .GroupBy(p => p.IncomingTrustUkprn)
                    .Where(p => p.Key != null)
                    .Select(g => new
                    {
                        UkprnInt = g.Key,
                        Conversions = g.Count(p => p.Type == ProjectType.Conversion),
                        Transfers = g.Count(p => p.Type == ProjectType.Transfer)
                    })
                    .ToListAsync(cancellationToken);

                var matGroups = await new FormAMatQuery(true)
                    .Apply(baseQ)
                    .GroupBy(p => new { NewTrustReferenceNumber = p.NewTrustReferenceNumber!, NewTrustName = p.NewTrustName! })
                    .Select(g => new
                    {
                        Key = g.Key.NewTrustReferenceNumber,
                        Name = g.Key.NewTrustName,
                        GroupIdentifier = g.Key.NewTrustReferenceNumber,
                        Conversions = g.Count(p => p.Type == ProjectType.Conversion),
                        Transfers = g.Count(p => p.Type == ProjectType.Transfer)
                    })
                    .ToListAsync(cancellationToken);

                var ukprnStrings = nonMatGroups
                    .Select(x => x.UkprnInt!.Value.ToString())
                    .Distinct()
                    .ToList();

                var apiDtos = await trustsClient
                    .GetByUkprnsAllAsync(ukprnStrings, cancellationToken);

                var apiDtoDictionary = apiDtos == null
                    ? new Dictionary<string, TrustDto>()
                    : apiDtos.ToDictionary(dto => dto.Ukprn!, dto => dto);

                var nonMatResults = nonMatGroups
                    .Select(g =>
                    {
                        var uk = g.UkprnInt!.Value.ToString();
                        var dto = apiDtoDictionary.GetValueOrDefault(uk);

                        if (dto is null)
                            return null;

                        return new ListTrustsWithProjectsResultModel(
                            Identifier: dto.Ukprn!,
                            TrustName: dto.Name!.ToTitleCase(),
                            GroupIdentifier: dto.ReferenceNumber!,
                            ConversionCount: g.Conversions,
                            TransfersCount: g.Transfers);
                    })
                    .Where(r => r != null)
                    .Cast<ListTrustsWithProjectsResultModel>()
                    .ToList();

                var matResults = matGroups
                    .Select(g => new ListTrustsWithProjectsResultModel(
                        Identifier: g.Key,
                        TrustName: g.Name,
                        GroupIdentifier: g.Key,
                        ConversionCount: g.Conversions,
                        TransfersCount: g.Transfers));

                var all = nonMatResults
                    .Concat(matResults)
                    .OrderBy(r => r.TrustName)
                    .ToList();

                var page = all
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .ToList();

                var total = all.Count;

                return PaginatedResult<List<ListTrustsWithProjectsResultModel>>
                    .Success(page, total);

            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Exception in {Handler} for {@Request}",
                    nameof(ListAllTrustsWithProjectsQueryHandler),
                    request);

                return PaginatedResult<List<ListTrustsWithProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}