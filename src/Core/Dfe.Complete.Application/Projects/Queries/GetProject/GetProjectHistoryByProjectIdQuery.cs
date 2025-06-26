using MediatR;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using AutoMapper;
using Dfe.Complete.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectHistoryByProjectIdQuery(string ProjectId) : IRequest<Result<ProjectDto>>;

    public class GetProjectHistoryByProjectIdHandler(ICompleteRepository<Project> projectRepository,
        IMapper mapper,
        ILogger<GetProjectHistoryByProjectIdHandler> logger)
        : IRequestHandler<GetProjectHistoryByProjectIdQuery, Result<ProjectDto>>
    {
        public async Task<Result<ProjectDto>> Handle(GetProjectHistoryByProjectIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectId = new ProjectId(Guid.Parse(request.ProjectId));
                
                var result = await projectRepository
                    .Query()
                    .Where(p => p.Id == projectId)
                    .Include(p => p.Notes)
                    .Include(p => p.SignificantDateHistories)
                        .ThenInclude(ph => ph.User)
                    .Include(p => p.SignificantDateHistories)
                        .ThenInclude(ph => ph.Reason)
                    .FirstOrDefaultAsync(cancellationToken);

                result!.Notes = result.Notes.Where(n => n.NotableType == "SignificantDateHistoryReason").ToList();

                var projectDto = mapper.Map<ProjectDto>(result);

                return Result<ProjectDto>.Success(projectDto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectHistoryByProjectIdHandler), request);
                return Result<ProjectDto>.Failure(ex.Message);
            }

        }
    }
}