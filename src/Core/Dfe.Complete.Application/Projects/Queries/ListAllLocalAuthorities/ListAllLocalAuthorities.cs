using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.ProjectsByLocalAuthority;

public record ListAllProjectLocalAuthoritiesQuery(ProjectState? ProjectStatus, ProjectType? Type)
    : IRequest<Result<List<ListAllProjectLocalAuthoritiesQueryModel>>>;

public class ListAllLocalAuthorities() : IRequestHandler<ListAllProjectLocalAuthoritiesQuery, Result<List<ListAllProjectLocalAuthoritiesQueryModel>>>
{
    public Task<Result<List<ListAllProjectLocalAuthoritiesQueryModel>>> Handle(ListAllProjectLocalAuthoritiesQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}