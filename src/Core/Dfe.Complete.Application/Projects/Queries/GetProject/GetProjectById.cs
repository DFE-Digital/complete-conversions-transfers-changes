using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Common.Models;
using AutoMapper;
using Dfe.Complete.Application.Projects.Models;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectByIdQuery(ProjectId ProjectId) : IRequest<Result<ProjectDto?>>;

    public class GetProjectByIdQueryHandler(ICompleteRepository<Project> projectRepository,
         IMapper mapper)
        : IRequestHandler<GetProjectByIdQuery, Result<ProjectDto?>>
    {
        public async Task<Result<ProjectDto?>> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await projectRepository.GetAsync(p => p.Id == request.ProjectId);

                var projectDto = mapper.Map<ProjectDto?>(result);

                return Result<ProjectDto?>.Success(projectDto);
            }
            catch (Exception ex)
            {
                return Result<ProjectDto?>.Failure(ex.Message);
            }
        }
    }
}