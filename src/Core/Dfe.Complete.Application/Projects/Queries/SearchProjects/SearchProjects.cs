using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.SearchProjects
{ 
    public record SearchProjectsQuery(
       string SearchTerm,
       List<ProjectState> ProjectStates) : PaginatedRequest<PaginatedResult<List<ListAllProjectsResultModel>>>;

    public class SearchProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<SearchProjectsQuery, PaginatedResult<List<ListAllProjectsResultModel>>>
    {
        public async Task<PaginatedResult<List<ListAllProjectsResultModel>>> Handle(SearchProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var searchQuery = listAllProjectsQueryService
                    .ListAllProjects(new ProjectFilters(null, null, ProjectStatuses: request.ProjectStates), search: request.SearchTerm).AsQueryable();

                var itemCount = await searchQuery
                    .CountAsync(cancellationToken);

                var projectList = await searchQuery
                    .Skip(request.Page * request.Count).Take(request.Count)
                    .Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                       item.Project!,
                       item.Establishment
                   )).ToListAsync(cancellationToken); 

                return PaginatedResult<List<ListAllProjectsResultModel>>.Success(projectList, itemCount);
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ListAllProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}
