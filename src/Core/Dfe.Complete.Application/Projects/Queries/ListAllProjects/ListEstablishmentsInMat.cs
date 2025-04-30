using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListEstablishmentsInMatQuery(string ReferenceNumber) : PaginatedRequest<Result<ListMatResultModel>>;

    public class ListEstablishmentsInMatQueryHandler(IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<ListEstablishmentsInMatQuery, Result<ListMatResultModel>>
    {
        public async Task<Result<ListMatResultModel>> Handle(ListEstablishmentsInMatQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var projects = await listAllProjectsQueryService.ListAllProjects(ProjectState.Active, null, newTrustReferenceNumber: request.ReferenceNumber)
                    .ToListAsync(cancellationToken);
                
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