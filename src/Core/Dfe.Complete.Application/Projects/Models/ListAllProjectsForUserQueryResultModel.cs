using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectsForUserQueryResultModel(
    ProjectId ProjectId,
    Urn Urn,
    string SchoolOrAcademyName,
    ProjectType? ProjectType,
    bool IsFormAMat,
    string? IncomingTrustName,
    string? OutgoingTrustName,
    string LocalAuthority,
    DateOnly? ConversionOrTransferDate
)
{
    public static ListAllProjectsForUserQueryResultModel MapProjectAndEstablishmentToListAllProjectsForUserQueryResultModel(Project project,
            GiasEstablishment giasEstablishment, string? outgoingTrustName, string? incomingTrustName)
    {
        return new ListAllProjectsForUserQueryResultModel(project.Id,
            project.Urn,
            giasEstablishment.Name,
            project.Type,
            project.FormAMat,
            incomingTrustName,
            outgoingTrustName,
            giasEstablishment.LocalAuthorityName,
            project.SignificantDate
        );
    }
}