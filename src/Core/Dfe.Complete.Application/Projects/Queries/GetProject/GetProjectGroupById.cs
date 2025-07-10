using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MediatR;
using Dfe.Complete.Domain.ValueObjects;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectGroupByIdQuery(ProjectGroupId Id) : IRequest<Result<ProjectGroupDto>>;

    public class GetProjectGroupByIdQueryHandler(ICompleteRepository<ProjectGroup> projectGroupRepository,
        IMapper mapper,
        ILogger<GetProjectGroupByIdQueryHandler> logger)
        : IRequestHandler<GetProjectGroupByIdQuery, Result<ProjectGroupDto>>
    {
        public async Task<Result<ProjectGroupDto>> Handle(GetProjectGroupByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await projectGroupRepository.GetAsync(p => p.Id == request.Id);

                var projectGroupDto = mapper.Map<ProjectGroupDto>(result);

                return Result<ProjectGroupDto>.Success(projectGroupDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectGroupByIdQueryHandler), request);
                return Result<ProjectGroupDto>.Failure(ex.Message);
            }
        }
    }
}
