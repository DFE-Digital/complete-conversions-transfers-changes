using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Model;
using Dfe.Complete.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.ListAllProjects
{
    public record ListAllProjectsQuery(
        ProjectState? ProjectStatus,
        ProjectType? Type,
        int Page = 0,
        int Count = 20) : IRequest<Result<List<Model.ListAllProjectsResultModel>>>;

    public class ListAllProjectsQueryHandler(
        IListAllProjectsQueryService listAllProjectsQueryService)
        : IRequestHandler<ListAllProjectsQuery, Result<List<Model.ListAllProjectsResultModel>>>
    {
        public async Task<Result<List<Model.ListAllProjectsResultModel>>> Handle(ListAllProjectsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var result = await listAllProjectsQueryService
                    .ListAllProjects(request.ProjectStatus, request.Type)
                    .Skip(request.Page * request.Count).Take(request.Count)
                    .Select(item => new Model.ListAllProjectsResultModel(
                        item.Establishment.Name,
                        item.Project.Id,
                        item.Project.Urn,
                        item.Project.SignificantDate,
                        item.Project.State,
                        item.Project.Type,
                        item.Project.IncomingTrustUkprn == null,
                        item.Project.AssignedTo != null
                            ? $"{item.Project.AssignedTo.FirstName} {item.Project.AssignedTo.LastName}"
                            : null))
                    .ToListAsync(cancellationToken);
                return Result<List<Model.ListAllProjectsResultModel>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<List<Model.ListAllProjectsResultModel>>.Failure(ex.Message);
            }
        }
    }
}