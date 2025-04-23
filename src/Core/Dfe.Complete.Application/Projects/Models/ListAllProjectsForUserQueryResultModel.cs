using AutoMapper;
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
    DateOnly? ConversionOrTransferDate,
    User? AssignedTo
)
{
    public static ListAllProjectsForUserQueryResultModel MapProjectAndEstablishmentToListAllProjectsForUserQueryResultModel(Project project,
            GiasEstablishment giasEstablishment, string? outgoingTrustName, string? incomingTrustName)
    {
        if (project.AssignedTo is not null)
        {
            project.AssignedTo.Notes = null;
            project.AssignedTo.ProjectAssignedTos = null;
            project.AssignedTo.ProjectCaseworkers = null;
            project.AssignedTo.ProjectRegionalDeliveryOfficers = null;
        }
        
        return new ListAllProjectsForUserQueryResultModel(project.Id,
            project.Urn,
            giasEstablishment.Name,
            project.Type,
            project.FormAMat,
            incomingTrustName,
            outgoingTrustName,
            giasEstablishment.LocalAuthorityName,
            project.SignificantDate,
            project.AssignedTo
        );
    }
}