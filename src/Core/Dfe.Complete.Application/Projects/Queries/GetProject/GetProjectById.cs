using MediatR;
using Dfe.Complete.Domain.ValueObjects; 
using Dfe.Complete.Application.Common.Models;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Microsoft.EntityFrameworkCore;
using Dfe.Complete.Utils;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectByIdQuery(ProjectId ProjectId) : IRequest<Result<ProjectDto?>>;

    public class GetProjectByIdQueryHandler(IProjectReadRepository projectReadRepository,
         IMapper mapper)
        : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto?>>
    {
        public async Task<Result<ProjectDto?>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await new ProjectIdQuery(request.ProjectId)
                    .Apply(projectReadRepository.Projects.AsNoTracking())
                    .Include(p => p.GiasEstablishment)
                    .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"No project found for Id: {request.ProjectId.Value}.");

                var projectDto = mapper.Map<ProjectDto?>(result);
                projectDto!.EstablishmentName = result.GiasEstablishment?.Name;

                return Result<ProjectDto?>.Success(projectDto);
            }
            catch (Exception ex)
            {
                return Result<ProjectDto?>.Failure(ex.Message);
            }
        }
    }
}