using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;

namespace Dfe.Complete.Models
{
    public record TaskSaveAndReturnModel(ProjectDto Project, ProjectTeam CurrentUserTeam);
}
