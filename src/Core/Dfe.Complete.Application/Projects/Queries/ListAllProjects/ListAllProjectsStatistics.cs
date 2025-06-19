using Dfe.Complete.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    //public record ListAllProjectsStatisticsQuery
    //{
    //}
    //public class ListAllProjectsStatisticsQueryHandler(ILogger<ListAllProjectsStatisticsQueryHandler> logger) : IRequest<ListAllProjectsStatisticsResult>
    //{
    //    public Task<ListAllProjectsStatisticsResult> Handle(ListAllProjectsStatisticsQuery request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {
    //            // This is a placeholder implementation. Replace with actual logic to retrieve statistics.
    //            var result = new ListAllProjectsStatisticsResult
    //            {
    //                TotalProjects = 100,
    //                CompletedProjects = 75,
    //                InProgressProjects = 25
    //            };
    //            return Result<ListAllProjectsStatisticsResult>.Success(result);
    //        }
    //        catch (Exception e)
    //        {
    //            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(ListAllProjectsForTeamQueryHandler), request);
    //            return Result<ListAllProjectsStatisticsResult>.Failure(e.Message);
    //        }
    //    }
    //}
}
