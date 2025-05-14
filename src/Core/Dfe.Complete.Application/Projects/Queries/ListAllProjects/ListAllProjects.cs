using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsQuery(
        ProjectState? ProjectStatus,
        ProjectType? Type,
        AssignedToState? AssignedToState = null,
        int Page = 0,
        int Count = 20) : IRequest<Result<List<ListAllProjectsResultModel>>>;

    public class ListAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<ListAllProjectsQuery, Result<List<ListAllProjectsResultModel>>>
    {
        public async Task<Result<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projectList = await listAllProjectsQueryService
                    .ListAllProjects(request.ProjectStatus, request.Type, assignedToState: request.AssignedToState)
                    .ToListAsync(cancellationToken);

                var result = projectList
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