using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllMATsQuery() : PaginatedRequest<PaginatedResult<List<ListMatResultModel>>>;

    public class ListAllMATsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<ListAllMATsQuery, PaginatedResult<List<ListMatResultModel>>>
    {
        public async Task<PaginatedResult<List<ListMatResultModel>>> Handle(ListAllMATsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var allProjects = listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                    .AsEnumerable()
                    .ToList();
                
                var matProjects = allProjects.Where(p => p.Project.FormAMat);

                //Group mats by reference and form result model
                var mats = matProjects
                    .GroupBy(p => p.Project.NewTrustReferenceNumber)
                    .Select(projects => new ListMatResultModel(
                        projects.Key,
                        projects.First().Project.NewTrustName, 
                        projects
                    ))
                    .ToList();
                
                
                var allMATs = mats.OrderBy(r => r.trustName)
                    .ToList();
                
                var result = allMATs
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToList();

                return PaginatedResult<List<ListMatResultModel>>.Success(result, allMATs.Count);
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ListMatResultModel>>.Failure(ex.Message);
            }
        }
    }
}