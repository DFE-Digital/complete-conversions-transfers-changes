using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Models;

public record ProjectWithEstablishmentQueryModel(
    string? EstablishmentName,
    ProjectId ProjectId,
    Urn Urn,
    DateOnly? ConversionOrTransferDate,
    ProjectType? ProjectType,
    bool IsFormAMAT,
    string? RegionalDeliveryOfficer, 
    DateTime CreatedAt, 
    string? NewTrustName,
    string? NewTrustReferenceNumber,
    DateOnly? AdvisoryBoardDate,
    string? OutgoingTrustName,
    Ukprn? OutgoingTrustUkprn,
    AcademyOrderType AcademyOrderType,
    string? IncomingTrustName,
    Ukprn? IncomingTrustUkprn)
{
    public static ProjectWithEstablishmentQueryModel MapProjectAndEstablishmentToModel(Project project, GiasEstablishment? establishment, string incomingTrustName, string? outgoingTrustName)
    {
        return new ProjectWithEstablishmentQueryModel(
            establishment?.Name,
            project.Id,
            project.Urn,
            project.SignificantDate,
            project.Type,
            project.FormAMat,
            project.RegionalDeliveryOfficer?.FullName,  
            project.CreatedAt, 
            project.NewTrustName ?? incomingTrustName,
            project.NewTrustReferenceNumber,
            project.AdvisoryBoardDate,
            outgoingTrustName,
            project.OutgoingTrustUkprn, 
            project.DirectiveAcademyOrder == true ? AcademyOrderType.DirectiveAcademyOrder : AcademyOrderType.AcademyOrder,
            incomingTrustName,
            project.IncomingTrustUkprn
        );
    }
}