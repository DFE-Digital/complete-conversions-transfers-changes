using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Dfe.Complete.Utils;
using DfE.CoreLibs.Utilities.Constants;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListProjectsByMonth
{
    public record ListProjectsByMonthsQuery(
        DateOnly FromDate,
        DateOnly? ToDate,
        ProjectState? ProjectStatus,
        ProjectType? Type,
        int Page = 0,
        int Count = 20) : IRequest<PaginatedResult<List<ListProjectsByMonthResultModel>>>;

    public class ListAllProjectsByMonthsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService,
        ITrustsV4Client trustsClient,
        ILogger<ListAllProjectsByMonthsQueryHandler> logger)
        : IRequestHandler<ListProjectsByMonthsQuery, PaginatedResult<List<ListProjectsByMonthResultModel>>>
    {
        public async Task<PaginatedResult<List<ListProjectsByMonthResultModel>>> Handle(ListProjectsByMonthsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projectsQuery = listAllProjectsQueryService
                    .ListAllProjects(new ProjectFilters(request.ProjectStatus, request.Type, AssignedToState: AssignedToState.AssignedOnly, 
                    SignificantDateRange: new DateRangeFilter(request.FromDate, request.ToDate)))
                    .AsEnumerable();

                var projects = projectsQuery.Where(p => p.Project.SignificantDateProvisional == false).ToList(); // Confirmed && Inprogress

                var ukprns = projects.Select(p => p.Project.IncomingTrustUkprn?.Value.ToString()).ToList();
                var outgoingTrustUkprns = projects.Where(p => p.Project.OutgoingTrustUkprn != null).Select(p => p.Project.OutgoingTrustUkprn.Value.ToString()).ToList();
                var allUkprns = ukprns.Concat(outgoingTrustUkprns).Distinct();
                
                var trusts = await trustsClient.GetByUkprnsAllAsync(allUkprns, cancellationToken);

                var result = projects
                    .Skip(request.Page * request.Count)
                    .Take(request.Count)
                    .Select(item =>
                    {
                        var project = item.Project;

                        var confirmedDate = project.SignificantDate?.ToString(DateFormatConstants.MonthAndYearFormat);
                        var originalDate = project.SignificantDateHistories.Any()
                            ? project.SignificantDateHistories
                                .OrderBy(s => s.CreatedAt)
                                .FirstOrDefault()?.PreviousDate?.ToString(DateFormatConstants.MonthAndYearFormat)
                            : null;
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
                            item.Establishment.LocalAuthorityName,
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
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsByMonthsQueryHandler), request);
                return PaginatedResult<List<ListProjectsByMonthResultModel>>.Failure(ex.Message);
            }
        }
    }
}