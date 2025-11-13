using AutoMapper;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectGroupByGroupReferenceNumberQuery(string GroupReferenceNumber) : IRequest<Result<ProjectGroupDto>>;

    public class GetProjectGroupByGroupReferenceNumberQueryHandler(ICompleteRepository<ProjectGroup> projectGroupRepository,
        IMapper mapper,
        ILogger<GetProjectGroupByGroupReferenceNumberQueryHandler> logger)
        : IRequestHandler<GetProjectGroupByGroupReferenceNumberQuery, Result<ProjectGroupDto>>
    {
        public async Task<Result<ProjectGroupDto?>> Handle(GetProjectGroupByGroupReferenceNumberQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await projectGroupRepository.GetAsync(p => p.GroupIdentifier == request.GroupReferenceNumber);

                var projectGroupDto = mapper.Map<ProjectGroupDto?>(result);

                return Result<ProjectGroupDto?>.Success(projectGroupDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectGroupByGroupReferenceNumberQueryHandler), request);
                return Result<ProjectGroupDto?>.Failure(ex.Message);
            }

        }
    }
}