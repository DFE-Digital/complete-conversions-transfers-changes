using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects;

public record ListAllProjectForUserQuery (ProjectState? State, string UserAdId)
    : PaginatedRequest<PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>>;

public class ListAllProjectsForUser(
    IListAllProjectsForUserQueryService projectsForUserQueryService,
    ITrustsV4Client trustsClient, 
    ISender sender)
    : IRequestHandler<ListAllProjectForUserQuery, PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>>
{
    public async Task<PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>> Handle(
        ListAllProjectForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await sender.Send(new GetUserByAdIdQuery(request.UserAdId), cancellationToken);

            var projectsForUser = await projectsForUserQueryService.ListAllProjectsForUser(user.Value.Id, request.State)
                .ToListAsync(cancellationToken);

            var allProjectTrustUkPrns = projectsForUser
                .SelectMany(p => new[]
                {
                    p.Project?.IncomingTrustUkprn?.Value.ToString() ?? string.Empty,
                    p.Project?.OutgoingTrustUkprn?.Value.ToString() ?? string.Empty
                })
                .Where(ukPrn => !string.IsNullOrEmpty(ukPrn))
                .ToList();

            var allTrusts = await trustsClient.GetByUkprnsAllAsync(allProjectTrustUkPrns, cancellationToken);

            var result = projectsForUser.Select(p => new ListAllProjectsForUserQueryResultModel(
                    p.Project.Id,
                    p.Establishment.Urn,
                    p.Establishment.Name,
                    p.Project.Type,
                    p.Project.FormAMat,
                    IncomingTrustName: allTrusts
                        .FirstOrDefault(trust => trust.Ukprn == p.Project?.IncomingTrustUkprn?.Value.ToString())?.Name,
                    OutgoingTrustName: allTrusts
                        .FirstOrDefault(trust => trust.Ukprn == p.Project?.OutgoingTrustUkprn?.Value.ToString())?.Name,
                    p.Establishment.LocalAuthorityName,
                    p.Project.SignificantDate))
                .Skip(request.Page * request.Count)
                .ToList();

            return PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>.Success(result, projectsForUser.Count);
        }
        catch (Exception e)
        {
            return PaginatedResult<List<ListAllProjectsForUserQueryResultModel>>.Failure(e.Message);
        }
    }
}