using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ProjectsByRegion
{
    public record CountAllProjectsForRegionQuery(Region Region, ProjectState? ProjectStatus, ProjectType? Type) : IRequest<Result<int>>;

    public class CountAllProjectsForRegionQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<CountAllProjectsForRegionQuery, Result<int>>
    {
        public async Task<Result<int>> Handle(CountAllProjectsForRegionQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await listAllProjectsQueryService
                    .ListAllProjects(request.ProjectStatus, request.Type).ToListAsync(cancellationToken: cancellationToken);

                var count = result.Count(p => p.Project?.Region == request.Region);
                
                return Result<int>.Success(count);
            }
            catch (Exception ex)
            {
                return Result<int>.Failure(ex.Message);
            }
        }
    }
}