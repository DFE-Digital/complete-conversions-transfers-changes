using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Infrastructure.QueryServices;

internal class ListAllProjectsForUserQueryService(CompleteContext context) : IListAllProjectsForUserQueryService
{
    public IQueryable<ListAllProjectsQueryModel> ListAllProjectsForUser(UserId userId, ProjectState? projectStatus)
    {
        var query = context.Projects
            .Include(p => p.AssignedTo)
            .Where(project => project.State == projectStatus)
            .Where(project => project.AssignedToId != null && project.AssignedToId == userId)
            .Join(context.GiasEstablishments,
                project => project.Urn, establishment => establishment.Urn,
                (project, establishment) => new ListAllProjectsQueryModel(project, establishment));

        return query;
    }
}