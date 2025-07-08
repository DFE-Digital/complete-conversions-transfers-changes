using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsConvertingQuery(bool WithAcademyUrn) : PaginatedRequest<PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>>;

    public class ListAllProjectsConvertingQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService, ICompleteRepository<GiasEstablishment> establishmentRepo, ICompleteRepository<ConversionTasksData> taskDataRepo)
        : IRequestHandler<ListAllProjectsConvertingQuery, PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>>
    {
        public async Task<PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>> Handle(ListAllProjectsConvertingQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var orderBy = new OrderProjectQueryBy() { Field = OrderProjectByField.SignificantDate, Direction = OrderByDirection.Ascending };
                
                var projectsQuery = listAllProjectsQueryService
                    .ListAllProjects(new ProjectFilters(ProjectState.Active, ProjectType.Conversion, WithAcademyUrn: request.WithAcademyUrn), null, orderBy);
                var totalProjectCount = await projectsQuery.CountAsync(cancellationToken);
                
                var convertingProjects = await projectsQuery
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .ToListAsync(cancellationToken);
                
                var taskDataIds = convertingProjects.Select(p => p.Project.TasksDataId).ToList();
                var taskData = await taskDataRepo.FetchAsync(t => taskDataIds.Contains(t.Id));
               
               List<GiasEstablishment> giasEstablishments = new List<GiasEstablishment>();
               
               if (request.WithAcademyUrn)
               {
                   var academuUrns = convertingProjects.Select(p => p.Project.AcademyUrn).Distinct();
                   giasEstablishments = (await establishmentRepo.FetchAsync(e => academuUrns.Contains(e.Urn), cancellationToken)).ToList();
               }
     
                var projects = convertingProjects
                    .Select(item =>
                        {
                            var academyName =
                                taskData.FirstOrDefault(t => t.Id == item.Project.TasksDataId)?.AcademyDetailsName ??
                                giasEstablishments.FirstOrDefault(e => e.Urn == item.Project.AcademyUrn)?.Name;
                            
                            return new ListAllProjectsConvertingQueryResultModel(item.Project.Id,
                                item.Establishment?.Name, 
                                item.Establishment?.Urn?.Value, 
                                item.Project.SignificantDate,
                                academyName,
                                item.Project.AcademyUrn?.Value);
                        }
                    );

                return PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>.Success(projects, totalProjectCount);
            }
            catch (Exception ex)
            {
                return PaginatedResult<IEnumerable<ListAllProjectsConvertingQueryResultModel>>.Failure(ex.Message);
            }
        }
    }
}