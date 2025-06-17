using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListEstablishmentsInMatQuery(string ReferenceNumber) : PaginatedRequest<Result<ListMatResultModel>>;

    public class ListEstablishmentsInMatQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService, ILogger<ListEstablishmentsInMatQueryHandler> logger)
        : IRequestHandler<ListEstablishmentsInMatQuery, Result<ListMatResultModel>>
    {
        public async Task<Result<ListMatResultModel>> Handle(ListEstablishmentsInMatQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projects = await listAllProjectsQueryService.ListAllProjects(
                    new ProjectFilters(ProjectState.Active, null, NewTrustReferenceNumber: request.ReferenceNumber)
                    )
                    .ToListAsync(cancellationToken);

                if (!projects.Any())
                {
                    return Result<ListMatResultModel>.Failure("No projects found");
                }

                var firstProject = projects.First();

                var result = new ListMatResultModel(
                    firstProject.Project.NewTrustReferenceNumber,
                    firstProject.Project.NewTrustName,
                    projects.Select(model =>
                        ListAllProjectsResultModel.MapProjectAndEstablishmentToListAllProjectResultModel(model.Project,
                            model.Establishment))
                );

                return Result<ListMatResultModel>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(ListEstablishmentsInMatQueryHandler), request);
                return Result<ListMatResultModel>.Failure(ex.Message);
            }
        }
    }
}