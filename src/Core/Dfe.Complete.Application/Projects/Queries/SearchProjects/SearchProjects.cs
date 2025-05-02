using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models; 
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.SearchProjects
{ 
    public record SearchProjectsQuery(
       ProjectState? ProjectStatus, 
       string SearhchTerm,
       int Page = 0,
       int Count = 20) : IRequest<Result<List<ListAllProjectsResultModel>>>;

    public class SearchProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<SearchProjectsQuery, Result<List<ListAllProjectsResultModel>>>
    {
        public async Task<Result<List<ListAllProjectsResultModel>>> Handle(SearchProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projectList = await listAllProjectsQueryService
                    .SearchProjects(request.ProjectStatus, request.SearhchTerm, request.Count)
                    .ToListAsync(cancellationToken);

                var filteredProjectList = request.ProjectStatus == ProjectState.Active
                    ? projectList.Where(p => p.Project?.AssignedTo != null)
                    : projectList;

                var result = filteredProjectList
                    .Skip(request.Page * request.Count).Take(request.Count)
                    .Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                        item.Project!,
                        item.Establishment
                    ))
                    .ToList();
                return Result<List<ListAllProjectsResultModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<ListAllProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}
