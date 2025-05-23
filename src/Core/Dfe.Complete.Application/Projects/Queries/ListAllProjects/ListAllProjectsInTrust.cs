using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Utils;
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
                string? incomingTrustUkprnFilterValue = null;
                string? newTrustReferenceNumberFilterValue = request.IsFormAMat ? request.Identifier : null;
                string? trustName = string.Empty;

                if (!request.IsFormAMat)
                {
                    var trust = await trustsClient.GetTrustByUkprn2Async(request.Identifier, cancellationToken) ?? throw new NotFoundException($"Trust with UKPRN {request.Identifier} not found.");
                    trustName = trust.Name;
                    incomingTrustUkprnFilterValue = trust.Ukprn;
                }

                var allProjects = listAllProjectsQueryService.ListAllProjects(Domain.Enums.ProjectState.Active, null,
                    newTrustReferenceNumber: newTrustReferenceNumberFilterValue, incomingTrustUkprn: incomingTrustUkprnFilterValue);

                if (request.IsFormAMat && allProjects.Any())
                    trustName = allProjects.First().Project.NewTrustName;

                var projectsQuery = allProjects
                    .Paginate(request.Page, request.Count)
                    .Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                        item.Project,
                        item.Establishment
                    ));

                var projects = await projectsQuery.ToListAsync(cancellationToken);
                var count = await allProjects.CountAsync(cancellationToken);

                var result = new ListAllProjectsInTrustResultModel(trustName ?? string.Empty, projects);

                return PaginatedResult<ListAllProjectsInTrustResultModel>.Success(result, count);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForUserQueryHandler), request);
                return PaginatedResult<ListAllProjectsInTrustResultModel>.Failure(ex.Message);
            }
        }
    }
}