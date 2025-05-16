using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Common.Models;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectByUrnQuery(Urn Urn) : IRequest<Result<ProjectDto?>>;

    public class GetProjectByUrnQueryHandler(ICompleteRepository<Project> projectRepository,
         IMapper mapper,
         ILogger<GetProjectByUrnQueryHandler> logger)
        : IRequestHandler<GetProjectByUrnQuery, Result<ProjectDto?>>
    {
        public async Task<Result<ProjectDto?>> Handle(GetProjectByUrnQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await projectRepository.GetAsync(p => p.Urn == request.Urn);

                var projectDto = mapper.Map<ProjectDto?>(result);

                return Result<ProjectDto?>.Success(projectDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectByUrnQueryHandler), request);
                return Result<ProjectDto?>.Failure(ex.Message);
            }
        }
    }
}