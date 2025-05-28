using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsInTrustQuery(string? Identifier, bool IsFormAMat) : PaginatedRequest<PaginatedResult<ListAllProjectsInTrustResultModel>>;

    public class ListAllProjectsInTrustQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService, ITrustsV4Client trustsClient, ILogger<ListAllProjectsInTrustQueryHandler> logger)
        : IRequestHandler<ListAllProjectsInTrustQuery, PaginatedResult<ListAllProjectsInTrustResultModel>>
    {
        public async Task<PaginatedResult<ListAllProjectsInTrustResultModel>> Handle(ListAllProjectsInTrustQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allProjects = await listAllProjectsQueryService.ListAllProjects(Domain.Enums.ProjectState.Active, null)
                    .ToListAsync(cancellationToken);

                var selectedProjects = new List<ListAllProjectsQueryModel>();
                var trustName = string.Empty;

                if (!request.IsFormAMat)
                {
                    var trust = await trustsClient.GetTrustByUkprn2Async(request.Identifier);
                    selectedProjects = allProjects.Where(p => p.Project.IncomingTrustUkprn == trust.Ukprn).ToList();
                    trustName = trust.Name;
                }
                else
                {
                    selectedProjects = allProjects.Where(p => p.Project.NewTrustReferenceNumber == request.Identifier).ToList();
                    trustName = selectedProjects.Any() ? selectedProjects.First().Project.NewTrustName : string.Empty;
                }

                selectedProjects = selectedProjects
                    .OrderBy(p => p.Project.SignificantDate)
                        .ThenBy(p => p.Establishment.Name)
                    .ToList();

                var projects = selectedProjects
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .Select(item =>  ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                        item.Project,
                        item.Establishment
                    ));

                var result = new ListAllProjectsInTrustResultModel(trustName, projects);

                return PaginatedResult<ListAllProjectsInTrustResultModel>.Success(result, selectedProjects.Count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForUserQueryHandler), request);
                return PaginatedResult<ListAllProjectsInTrustResultModel>.Failure(ex.Message);
            }
        }
    }
}