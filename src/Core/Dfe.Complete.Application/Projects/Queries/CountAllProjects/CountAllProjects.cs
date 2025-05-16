using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.CountAllProjects
{
    public record CountAllProjectsQuery(
        ProjectState? ProjectStatus,
        ProjectType? Type,
        AssignedToState? AssignedToState = null,
        string? Search = "") : IRequest<Result<int>>;

    public class CountAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<CountAllProjectsQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(CountAllProjectsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await listAllProjectsQueryService
                    .ListAllProjects(request.ProjectStatus, request.Type, search: request.Search, assignedToState: request.AssignedToState)
                    .CountAsync(cancellationToken);

                return Result<int>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure(ex.Message);
            }
        }
    }
}