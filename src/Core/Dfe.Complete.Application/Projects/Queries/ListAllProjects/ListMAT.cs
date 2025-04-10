using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListMatQuery(string referenceNumber) : PaginatedRequest<Result<ListMatResultModel>>;

    public class ListMatQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<ListMatQuery, Result<ListMatResultModel>>
    {
        public async Task<Result<ListMatResultModel>> Handle(ListMatQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projects = listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null)
                    .AsEnumerable()
                    .Where(p => p.Project.NewTrustReferenceNumber == request.referenceNumber)
                    .ToList();
                
                if (!projects.Any())
                {
                    return Result<ListMatResultModel>.Failure("No projects found");
                }

                var firstProject = projects.First();

                var result = new ListMatResultModel(
                    firstProject.Project.NewTrustReferenceNumber,
                    firstProject.Project.NewTrustName,
                    projects
                );

                return Result<ListMatResultModel>.Success(result);

            }
            catch (Exception ex)
            {
                return Result<ListMatResultModel>.Failure(ex.Message);
            }
        }
    }
}