using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsQuery(
        ProjectState? ProjectStatus,
        ProjectType? Type,
        int Page = 0,
        int Count = 20) : IRequest<Result<List<ListAllProjectsResultModel>>>;

    public class ListAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<ListAllProjectsQuery, Result<List<ListAllProjectsResultModel>>>
    {
        public async Task<Result<List<ListAllProjectsResultModel>>> Handle(ListAllProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await listAllProjectsQueryService
                    .ListAllProjects(request.ProjectStatus, request.Type)
                    .Skip(request.Page * request.Count).Take(request.Count)
                    .Select(item => new ListAllProjectsResultModel(
                        item.Establishment.Name,
                        item.Project.Id,
                        item.Project.Urn,
                        item.Project.SignificantDate,
                        item.Project.State,
                        item.Project.Type,
                        item.Project.FormAMat,
                        item.Project.AssignedTo != null
                            ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}"
                            : null,
                        item.Establishment.LocalAuthorityName,
                        item.Project.Team,
                        item.Project.CompletedAt
                    ))
                    .ToListAsync(cancellationToken);
                return Result<List<ListAllProjectsResultModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<ListAllProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}