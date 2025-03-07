using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Interfaces;

public interface IListAllProjectsForUserQueryService
{
    IQueryable<ListAllProjectsQueryModel> ListAllProjectsForUser(UserId userId, ProjectState? projectStatus);
}