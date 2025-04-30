using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllMaTsQuery(ProjectState Status) : PaginatedRequest<PaginatedResult<List<ListMatResultModel>>>;

    public class ListAllMaTsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        IEstablishmentsV4Client establishmentsClient)
        : IRequestHandler<ListAllMaTsQuery, PaginatedResult<List<ListMatResultModel>>>
    {
        public async Task<PaginatedResult<List<ListMatResultModel>>> Handle(ListAllMaTsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var matProjects = await listAllProjectsQueryService
                    .ListAllProjects(request.Status, null, isFormAMat: true)
                    .ToListAsync(cancellationToken);

                // Assigned to was causing issues in integration tests + it's not needed in this response 
                matProjects = matProjects.Select(p =>
                {
                    if (p.Project.AssignedTo != null)
                    {
                        p.Project.AssignedTo.Notes = null;
                        p.Project.AssignedTo.ProjectAssignedTos = null;
                        p.Project.AssignedTo.ProjectCaseworkers = null;
                        p.Project.AssignedTo.ProjectRegionalDeliveryOfficers = null;
                    }
                    return p;
                }).ToList();

                var urns = matProjects.Select(project => project.Project.Urn.Value).Distinct().ToList();

                var academiesEstablishments = await establishmentsClient.GetByUrns2Async(urns, cancellationToken);
                var establishments = academiesEstablishments
                    .Select(r => new GiasEstablishment
                    {
                        Urn = new Urn(int.Parse(r.Urn)),
                        Name = r.Name
                    });

                //Group mats by reference and form result model
                var allMATs = matProjects
                    .GroupBy(p => p.Project.NewTrustReferenceNumber)
                    .Select(group =>
                    {
                        var trustName = group.OrderBy(p => p.Project.CreatedAt).First().Project.NewTrustName;

                        var projectModels = group.Select(p =>
                        {
                            var project = p.Project;
                            var matchingEstablishment = establishments.First(e => e.Urn.Value == project.Urn.Value);

                            return new ListAllProjectsQueryModel(project, matchingEstablishment);
                        }).OrderByDescending(p => p.Establishment.Name);

                        return new ListMatResultModel(group.Key, trustName, projectModels);
                    })
                    .OrderBy(r => r.trustName)
                    .ToList();

                var result = allMATs
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .ToList();

                return PaginatedResult<List<ListMatResultModel>>.Success(result, allMATs.Count);
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ListMatResultModel>>.Failure(ex.Message);
            }
        }
    }
}