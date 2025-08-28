using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Application.Common.Models;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using Microsoft.Extensions.Logging;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectByUrnQuery(Urn Urn) : IRequest<Result<ProjectDto?>>;

    public class GetProjectByUrnQueryHandler(IProjectReadRepository projectReadRepository,
         IMapper mapper,
         ILogger<GetProjectByUrnQueryHandler> logger)
        : IRequestHandler<GetProjectByUrnQuery, Result<ProjectDto?>>
    {
        public async Task<Result<ProjectDto?>> Handle(GetProjectByUrnQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await new ProjectUrnQuery(request.Urn)
                    .Apply(projectReadRepository.Projects)
                    .FirstOrDefaultAsync(cancellationToken);

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