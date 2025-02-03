using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjectsByRegion;
public record ListAllProjectsByRegionQuery(ProjectState? ProjectStatus, ProjectType? Type) : IRequest<Result<List<ListAllProjectsByRegionResultModel>>>;

public class ListAllProjectsByRegionQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService)
    : IRequestHandler<ListAllProjectsByRegionQuery, Result<List<ListAllProjectsByRegionResultModel>>>

{
    public async Task<Result<List<ListAllProjectsByRegionResultModel>>> Handle(ListAllProjectsByRegionQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var projectsByRegion = await listAllProjectsQueryService
                .ListAllProjects(request.ProjectStatus, request.Type)
                .GroupBy(p => p.Project.Region).ToListAsync(cancellationToken);
            
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}