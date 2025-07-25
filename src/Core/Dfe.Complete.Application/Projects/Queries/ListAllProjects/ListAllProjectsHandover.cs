using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsHandoverQuery(
        ProjectState? ProjectStatus = ProjectState.Inactive, 
        OrderProjectQueryBy? OrderBy = null,
        int Page = 0,
        int Count = 20) : IRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

    public class ListAllProjectsHandoverQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        ITrustsV4Client trustsClient,
        ILogger<ListAllProjectsQueryHandler> logger)
        : IRequestHandler<ListAllProjectsHandoverQuery, PaginatedResult<List<ListAllProjectsResultModel>>>
    {
        public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsHandoverQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projectList = await listAllProjectsQueryService
                   .ListAllProjects(new ProjectFilters(request.ProjectStatus, null), orderBy: request.OrderBy)
                   .Skip(request.Page * request.Count)
                   .Take(request.Count)
                   .ToListAsync(cancellationToken);
                var allProjectTrustUkPrns = projectList
                .SelectMany(p => new[]
                {
                   p.Project?.IncomingTrustUkprn?.Value.ToString() ?? string.Empty,
                   p.Project?.OutgoingTrustUkprn?.Value.ToString() ?? string.Empty
                })
                .Where(ukPrn => !string.IsNullOrEmpty(ukPrn))
                .Distinct()
                .ToList();

                var allTrusts = await GetTrustsByUkprns(allProjectTrustUkPrns, cancellationToken);

                var result = projectList
                    .Select(p =>
                    {
                        p.Project.NewTrustName = p.Project.FormAMat ? p.Project.NewTrustName : allTrusts?.FirstOrDefault(trust => trust.Ukprn == p.Project?.IncomingTrustUkprn?.Value.ToString())?.Name;
                        return ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                                p.Project,
                                p.Establishment!);
                    })
                    .ToList();
                return PaginatedResult<List<ListAllProjectsResultModel>>.Success(result, projectList.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsQueryHandler), request);
                return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(ex.Message);
            }
        }
        private async Task<ObservableCollection<TrustDto>> GetTrustsByUkprns(List<string> allProjectTrustUkPrns, CancellationToken cancellationToken)
            => (allProjectTrustUkPrns == null || allProjectTrustUkPrns.Count == 0) ?
                [] : await trustsClient.GetByUkprnsAllAsync(allProjectTrustUkPrns, cancellationToken);

    }
}