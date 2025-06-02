using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.CountAllProjects
{
    public record CountAllProjectsQuery(
        ProjectState? ProjectStatus,
        ProjectType? Type,
        AssignedToState? AssignedToState = null) : IRequest<Result<int>>;

    public class CountAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        ILogger<CountAllProjectsQueryHandler> logger)
        : IRequestHandler<CountAllProjectsQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(CountAllProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await listAllProjectsQueryService
                    .ListAllProjects(
                        new ProjectFilters(request.ProjectStatus, request.Type, AssignedToState: request.AssignedToState))
                    .CountAsync(cancellationToken);

                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(CountAllProjectsQueryHandler), request);
                return Result<int>.Failure(ex.Message);
            }
        }
    }
}