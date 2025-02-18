using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsInTrustQuery(string Ukprn) : PaginatedRequest<PaginatedResult<ListAllProjectsInTrustResultModel>>;

    public class ListAllProjectsInTrustQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService, ITrustsV4Client trustsClient)
        : IRequestHandler<ListAllProjectsInTrustQuery, PaginatedResult<ListAllProjectsInTrustResultModel>>
    {
        public async Task<PaginatedResult<ListAllProjectsInTrustResultModel>> Handle(ListAllProjectsInTrustQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var trust = await trustsClient.GetTrustByUkprn2Async(request.Ukprn);

                var allProjects = listAllProjectsQueryService.ListAllProjects(Domain.Enums.ProjectState.Active, null)
                                                                                    .AsEnumerable()
                                                                                        .Where(q => q.Project.IncomingTrustUkprn.Value.ToString() == trust.Ukprn)
                                                                                            .OrderBy(p => p.Project.SignificantDate)
                                                                                                .ThenBy(p => p.Establishment.Name)
                                                                                        .ToList();

                var listAllProjectsResult = allProjects
                                                        .Skip(request.Page * request.Count)
                                                        .Take(request.Count)
                                                        .Select(item => new ListAllProjectsResultModel(
                                                            item.Establishment.Name,
                                                            item.Project.Id,
                                                            item.Project.Urn,
                                                            item.Project.SignificantDate,
                                                            item.Project.State,
                                                            item.Project.Type,
                                                            item.Project.IncomingTrustUkprn == null,
                                                            item.Project.AssignedTo != null
                                                                ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}" : null))
                                                        .ToList();

                var result = new ListAllProjectsInTrustResultModel(trust.Name, listAllProjectsResult);

                return PaginatedResult<ListAllProjectsInTrustResultModel>.Success(result, listAllProjectsResult.Count);
            }
            catch (Exception ex)
            {
                return PaginatedResult<ListAllProjectsInTrustResultModel>.Failure(ex.Message);
            }
        }
    }
}