using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Models;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsInTrustQuery(string? Identifier, bool IsFormAMat) : PaginatedRequest<PaginatedResult<ListAllProjectsInTrustResultModel>>;

    public class ListAllProjectsInTrustQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService, ITrustsV4Client trustsClient)
        : IRequestHandler<ListAllProjectsInTrustQuery, PaginatedResult<ListAllProjectsInTrustResultModel>>
    {
        public async Task<PaginatedResult<ListAllProjectsInTrustResultModel>> Handle(ListAllProjectsInTrustQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allProjects = listAllProjectsQueryService.ListAllProjects(Domain.Enums.ProjectState.Active, null)
                    .AsEnumerable();
                    
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
                    .Select(item => new ListAllProjectsResultModel(
                        item.Establishment.Name,
                        item.Project.Id,
                        item.Project.Urn,
                        item.Project.SignificantDate,
                        item.Project.State,
                        item.Project.Type,
                        item.Project.FormAMat,
                        item.Project.AssignedTo != null
                            ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}"
                            : null));
                
                var result = new ListAllProjectsInTrustResultModel(trustName, projects);
                
                return PaginatedResult<ListAllProjectsInTrustResultModel>.Success(result, selectedProjects.Count);
            }
            catch (Exception ex)
            {
                return PaginatedResult<ListAllProjectsInTrustResultModel>.Failure(ex.Message);
            }
        }
    }
}