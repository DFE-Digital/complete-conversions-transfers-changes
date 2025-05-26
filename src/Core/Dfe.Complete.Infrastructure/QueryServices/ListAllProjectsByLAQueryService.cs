using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsWithLAsQueryService(CompleteContext context) : IListAllProjectsWithLAsQueryService
{
    public IQueryable<Project> ListAllProjects(
        ProjectState? projectStatus = null,
        ProjectType? projectType = null)
    {
        return context.Projects
            .Include(p => p.LocalAuthority)
            .Where(p => p.State == projectStatus && (projectType == null || p.Type == projectType));
    }

}