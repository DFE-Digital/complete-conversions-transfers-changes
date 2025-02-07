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
    string? AssignedToFullName)
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
            project.IncomingTrustUkprn == null,
            project.AssignedTo != null
                ? $"{project.AssignedTo.FirstName} {project.AssignedTo.LastName}"
                : null
        );
    }
    
    public static ListAllProjectsResultModel MapProjectAndEstablishmentToListAllProjectResultModel(ListAllProjectsQueryModel queryModel)
    {
        return new ListAllProjectsResultModel(
            queryModel.Establishment?.Name,
            queryModel.Project?.Id!,
            queryModel.Project.Urn,
            queryModel.Project.SignificantDate,
            queryModel.Project.State,
            queryModel.Project.Type,
            queryModel.Project.IncomingTrustUkprn == null,
            queryModel.Project.AssignedTo != null
                ? $"{queryModel.Project.AssignedTo.FirstName} {queryModel.Project.AssignedTo.LastName}"
                : null
        );
    }
    
    public static ListAllProjectsResultModel MapProjectAndEstablishmentToListAllProjectResultModel(Project project)
    {
        return new ListAllProjectsResultModel(
            null,
            project.Id,
            project.Urn,
            project.SignificantDate,
            project.State,
            project.Type,
            project.IncomingTrustUkprn == null,
            project.AssignedTo != null
                ? $"{project.AssignedTo.FirstName} {project.AssignedTo.LastName}"
                : null
        );
    }
};