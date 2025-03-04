using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllTrustsWithProjectsQuery() : PaginatedRequest<PaginatedResult<List<ListTrustsWithProjectsResultModel>>>;

    public class ListAllTrustsWithProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService, ITrustsV4Client trustsClient)
        : IRequestHandler<ListAllTrustsWithProjectsQuery, PaginatedResult<List<ListTrustsWithProjectsResultModel>>>
    {
        public async Task<PaginatedResult<List<ListTrustsWithProjectsResultModel>>> Handle(ListAllTrustsWithProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var allProjects = listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                    .AsEnumerable()
                    .Select(p => p.Project)
                    .ToList();

                var standardProjects = allProjects.Where(p => !p.FormAMat);
                var matProjects = allProjects.Where(p => p.FormAMat);

                // Get trusts related to projects
                var incomingTrustUkprns = standardProjects.Select(p => p.IncomingTrustUkprn.Value.ToString());
                var standardProjectsTrust = await trustsClient.GetByUkprnsAllAsync(incomingTrustUkprns);

                var trusts = standardProjectsTrust
                    .Select(item => new ListTrustsWithProjectsResultModel(
                        item.Ukprn,
                        item.Name,
                        item.ReferenceNumber,
                        standardProjects.Count(p => p.IncomingTrustUkprn?.ToString() == item.Ukprn && p.Type == ProjectType.Conversion),
                        standardProjects.Count(p => p.IncomingTrustUkprn?.ToString() == item.Ukprn && p.Type == ProjectType.Transfer)
                    ))
                    .ToList();
                
                
                //Group mats by reference and form result model
                var mats = matProjects
                    .GroupBy(p => p.NewTrustReferenceNumber)
                    .Select(trustReference => new ListTrustsWithProjectsResultModel(
                        trustReference.Key,
                        trustReference.First().NewTrustName, 
                        trustReference.Key,
                        trustReference.Count(p => p.Type == ProjectType.Conversion),
                        trustReference.Count(p => p.Type == ProjectType.Transfer)
                    ))
                    .ToList();

                var allTrusts = trusts.Concat(mats)
                    .OrderBy(r => r.trustName)
                    .ToList();

                var result = allTrusts
                .Skip(request.Page * request.Count)
                .Take(request.Count)
                .ToList();

                return PaginatedResult<List<ListTrustsWithProjectsResultModel>>.Success(result, allTrusts.Count);
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ListTrustsWithProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}