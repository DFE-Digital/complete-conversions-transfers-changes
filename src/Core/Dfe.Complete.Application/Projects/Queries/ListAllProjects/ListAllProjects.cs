using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsQuery(
        ProjectState? ProjectStatus,
        ProjectType? Type,
        AssignedToState? AssignedToState = null,
        OrderProjectQueryBy? OrderBy = null,
        int Page = 0,
        int Count = 20) : IRequest<Result<List<ListAllProjectsResultModel>>>;

    public class ListAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        ILogger<ListAllProjectsQueryHandler> logger)
        : IRequestHandler<ListAllProjectsQuery, Result<List<ListAllProjectsResultModel>>>
    {
        public async Task<Result<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projectList = await listAllProjectsQueryService
                    .ListAllProjects(
                        new ProjectFilters(request.ProjectStatus, request.Type, AssignedToState: request.AssignedToState),
                        orderBy: request.OrderBy)
                    .Skip(request.Page * request.Count).Take(request.Count)
                    .Select(item => ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(
                        item.Project!,
                        item.Establishment
                    ))
                    .ToListAsync(cancellationToken);
                return Result<List<ListAllProjectsResultModel>>.Success(projectList);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsQueryHandler), request);
                return Result<List<ListAllProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}