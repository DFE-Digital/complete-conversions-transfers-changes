namespace Dfe.Complete.Application.Projects.Models;

public record ListProjectsGroupsModel(string groupId, string groupName, string groupIdentifier, int trustUkprn, string includedEstablishments);