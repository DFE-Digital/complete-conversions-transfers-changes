using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Domain.Enums;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllTrustsWithProjectsQuery() : PaginatedRequest<PaginatedResult<List<ListTrustsWithProjectsResultModel>>>;


    public class ListAllTrustsWithProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService, ITrustsV4Client trustsClient)
        : IRequestHandler<ListAllTrustsWithProjectsQuery, PaginatedResult<List<ListTrustsWithProjectsResultModel>>>
    {
        public async Task<PaginatedResult<List<ListTrustsWithProjectsResultModel>>> Handle(ListAllTrustsWithProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var allProjects = listAllProjectsQueryService.ListAllProjects(null, null);
                                                                                    //.Skip(request.Page * request.Count)
                                                                                    //.Take(request.Count)
                                                                                    //.OrderBy(p => p.Project.CreatedAt);

                var projects = allProjects.Select(p => p.Project).ToList();
                var incomingTrustUkprns = projects.Select(p => p.IncomingTrustUkprn.Value.ToString()).ToList();

                var trusts = await trustsClient.GetByUkprnsAllAsync(incomingTrustUkprns);

                var result = trusts.Select(item =>
                    {
                        var _projects = projects.Where(p => p.IncomingTrustUkprn.Value.ToString() == item.Ukprn);

                        var conversionsCount = _projects.Count(p => p.Type == ProjectType.Conversion);
                        var transfersCount = _projects.Count(p => p.Type == ProjectType.Transfer);

                        return new ListTrustsWithProjectsResultModel(item.Ukprn, item.Name, item.ReferenceNumber, conversionsCount, transfersCount);
                    })
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .OrderBy(p => p.trustName)
                    .ToList();


                return PaginatedResult<List<ListTrustsWithProjectsResultModel>>.Success(result, trusts.Count);
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ListTrustsWithProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}