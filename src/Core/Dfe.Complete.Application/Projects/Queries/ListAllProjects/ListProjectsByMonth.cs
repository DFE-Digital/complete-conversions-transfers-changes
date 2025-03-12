using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Application.Projects.Queries.ListProjectsByMonth
{
    public record ListProjectsByMonthQuery(
        int Month,
        int Year,
        ProjectState? ProjectStatus,
        ProjectType? Type,
        int Page = 0,
        int Count = 20) : IRequest<PaginatedResult<List<ListProjectsByMonthResultModel>>>;

    public class ListAllProjectsByMonthQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        ITrustsV4Client trustsClient)
        : IRequestHandler<ListProjectsByMonthQuery, PaginatedResult<List<ListProjectsByMonthResultModel>>>
    {
        public async Task<PaginatedResult<List<ListProjectsByMonthResultModel>>> Handle(ListProjectsByMonthQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projects =  await listAllProjectsQueryService
                    .ListAllProjects(request.ProjectStatus, request.Type)
                    .Where(p => p.Project.SignificantDate.Value.Month == request.Month && p.Project.SignificantDate.Value.Year == request.Year)
                    .ToListAsync(cancellationToken);

                var ukprns = projects.Select(p => p.Project.IncomingTrustUkprn.Value.ToString());
                var outgoingTrustUkprns = projects.Where(p => p.Project.OutgoingTrustUkprn != null).Select(p => p.Project.OutgoingTrustUkprn.Value.ToString());
                var allUkprns = ukprns.Concat(outgoingTrustUkprns);
                
                var trusts = await trustsClient.GetByUkprnsAllAsync(allUkprns, cancellationToken);

                var result = projects
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .Select(item =>
                    {
                        var project = item.Project;
                        
                        var confirmedDate = project.SignificantDate.Value.ToString("MMM yyyy");
                        var originalDate = project.SignificantDateHistories.Any() ? item.Project.SignificantDateHistories.FirstOrDefault()?.PreviousDate.Value.ToString("MMM yyyy") : null;

                        var isMatProject = project.FormAMat;

                        var incomingTrust = isMatProject
                            ? project.NewTrustName
                            : trusts.FirstOrDefault(t => t.Ukprn == project.IncomingTrustUkprn)?.Name;
                        
                        var outgoingTrust = trusts.FirstOrDefault(t => t.Ukprn == project.OutgoingTrustUkprn)?.Name;
                        
                        var allConditions = project.AllConditionsMet.Value ? "Yes" : "Not yet";

                        var confirmedAndOriginalDate = string.IsNullOrEmpty(originalDate) ? confirmedDate : $"{confirmedDate} ({originalDate})";
                        
                        return new ListProjectsByMonthResultModel(
                            item.Establishment.Name,
                            project.Region.ToDisplayDescription(),
                            project.LocalAuthority?.Name,
                            outgoingTrust,
                            project.Id,
                            project.Urn,
                            incomingTrust,
                            allConditions,
                            confirmedAndOriginalDate,
                            project.Type);
                    })
                    .ToList();
                
                return PaginatedResult<List<ListProjectsByMonthResultModel>>.Success(result, projects.Count);
            }
            catch (Exception ex)
            {
                return PaginatedResult<List<ListProjectsByMonthResultModel>>.Failure(ex.Message);
            }
        }
    }
}