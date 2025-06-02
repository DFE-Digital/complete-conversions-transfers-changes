using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectsForUserQuery(
    ProjectState? State,
    string UserAdId,
    ProjectUserFilter ProjectUserFilter,
    OrderProjectQueryBy? OrderProjectQueryBy)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>>;

public class ListAllProjectsForUserQueryHandler(
    IListAllProjectsQueryService listAllProjectsQueryService,
    ITrustsV4Client trustsClient,
    ISender sender,
    ILogger<ListAllProjectsForUserQueryHandler> logger)
    : IRequestHandler<ListAllProjectsForUserQuery, PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>> Handle(
        ListAllProjectsForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await sender.Send(new GetUserByAdIdQuery(request.UserAdId), cancellationToken);
            if (!user.IsSuccess || user.Value == null)
                throw new NotFoundException("User not found.");

            var assignedTo = request.ProjectUserFilter == ProjectUserFilter.AssignedTo ? user.Value?.Id : null;
            var createdBy = request.ProjectUserFilter == ProjectUserFilter.CreatedBy ? user.Value?.Id : null;
            var projectsForUser = await listAllProjectsQueryService
                .ListAllProjects(new ProjectFilters(request.State, null, AssignedToUserId: assignedTo, CreatedByUserId: createdBy),
                    orderBy: request.OrderProjectQueryBy)
                .ToListAsync(cancellationToken);

            var allProjectTrustUkPrns = projectsForUser
                .SelectMany<ListAllProjectsQueryModel, string>(p =>
                [
                    p.Project?.IncomingTrustUkprn?.Value.ToString() ?? string.Empty,
                    p.Project?.OutgoingTrustUkprn?.Value.ToString() ?? string.Empty
                ])
                .Where(ukPrn => !string.IsNullOrEmpty(ukPrn))
                .ToList();

            var allTrusts = await trustsClient.GetByUkprnsAllAsync(allProjectTrustUkPrns, cancellationToken);
            var result = projectsForUser
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .Select(p => ListAllProjectsForUserQueryResultModel
                    .MapProjectAndEstablishmentToListAllProjectsForUserQueryResultModel(
                        p.Project,
                        p.Establishment!,
                        outgoingTrustName: allTrusts.FirstOrDefault(trust =>
                            trust.Ukprn == p.Project?.OutgoingTrustUkprn?.Value.ToString())?.Name,
                        incomingTrustName: allTrusts.FirstOrDefault(trust =>
                            trust.Ukprn == p.Project?.IncomingTrustUkprn?.Value.ToString())?.Name))
                .ToList();

            return PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>.Success(result, projectsForUser.Count);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForUserQueryHandler),
                request);
            return PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>.Failure(e.Message);
        }
    }
}