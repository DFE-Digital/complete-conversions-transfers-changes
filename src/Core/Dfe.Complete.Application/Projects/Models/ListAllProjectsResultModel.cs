using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public record ListAllProjectsResultModel(
    string? EstablishmentName,
    ProjectId ProjectId,
    Urn Urn,
    DateOnly? ConversionOrTransferDate,
    ProjectState State,
    ProjectType? ProjectType,
    bool IsFormAMAT,
    string? AssignedToFullName,
    string? LocalAuthorityName,
    ProjectTeam? Team,
    DateTime? ProjectCompletionDate,
    Region? Region,
    string? LocalAuthorityNameFormatted,
    DateTime CreatedAt,
    string? RegionalDeliveryOfficerFullName,
    string? NewTrustName,
    string? NewTrustReferenceNumber,
    DateOnly? AdvisoryBoardDate
)
{
    public static ListAllProjectsResultModel MapProjectAndEstablishmentToListAllProjectResultModel(Project project, GiasEstablishment? establishment)
    {
        return new ListAllProjectsResultModel(
            establishment?.Name,
            project.Id,
            project.Urn,
            project.SignificantDate,
            project.State,
            project.Type,
            project.FormAMat,
            project.AssignedTo?.FullName,
            project.LocalAuthority?.Name,
            project.Team,
            project.CompletedAt,
            project.Region,
            establishment?.LocalAuthorityName,
            project.CreatedAt,
            project.RegionalDeliveryOfficer?.FullName,
            project.NewTrustName,
            project.NewTrustReferenceNumber,
            project.AdvisoryBoardDate
        );
    }
}