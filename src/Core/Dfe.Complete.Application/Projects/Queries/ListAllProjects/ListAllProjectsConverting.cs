using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsConvertingQuery(bool WithAcademyUrn) : PaginatedRequest<PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>>;

    public class ListAllProjectsConvertingQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService, ICompleteRepository<GiasEstablishment> establishmentRepo)
        : IRequestHandler<ListAllProjectsConvertingQuery, PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>>
    {
        public async Task<PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>> Handle(ListAllProjectsConvertingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var allProjects = await listAllProjectsQueryService.ListAllProjects(new ProjectFilters(Domain.Enums.ProjectState.Active, Domain.Enums.ProjectType.Conversion, WithAcademyUrn: request.WithAcademyUrn))
                    .ToListAsync(cancellationToken);

               var convertingProjects = allProjects.OrderBy(p => p.Project.SignificantDate);
               
               List<GiasEstablishment> giasEstablishments = new List<GiasEstablishment>();
               
               if (request.WithAcademyUrn)
               {
                   var academuUrns = convertingProjects.Select(p => p.Project.AcademyUrn).Distinct();
               
                   giasEstablishments = (await establishmentRepo.FetchAsync(e => academuUrns.Contains(e.Urn), cancellationToken)).ToList();
               }
     
                var projects = convertingProjects
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .Select(item =>
                        {
                            return new ListAllProjectsConvertingQueryResultModel(item.Project.Id,
                                item.Establishment.Name, 
                                item.Establishment.Urn.Value, 
                                item.Project.SignificantDate,
                                giasEstablishments.FirstOrDefault(e => e.Urn == item.Project.AcademyUrn)?.Name,
                                item.Project.AcademyUrn?.Value);
                        }
                    );

                return PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>.Success(projects, convertingProjects.Count());
            }
            catch (Exception ex)
            {
                return PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>.Failure(ex.Message);
            }
        }
    }
}