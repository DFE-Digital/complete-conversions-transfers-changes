using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Projects.Queries.GetProject;


public record GetProjectByTrnQuery(string NewTrustReferenceNumber) : IRequest<Result<GetProjectByTrnResponseDto?>>;

public class GetProjectByTrn(ICompleteRepository<Project> projectRepo, ILogger<GetProjectByTrn> logger) : IRequestHandler<GetProjectByTrnQuery, Result<GetProjectByTrnResponseDto?>>
{
    public async Task<Result<GetProjectByTrnResponseDto?>> Handle(GetProjectByTrnQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var project = await projectRepo.FindAsync(x => x.NewTrustReferenceNumber == request.NewTrustReferenceNumber, cancellationToken);

            var result = project != null ? new GetProjectByTrnResponseDto(project.Id.Value, project.NewTrustName) : null;

            return Result<GetProjectByTrnResponseDto?>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception for {Name} Request - {@Request}", nameof(GetProjectByTrn), request);
            return Result<GetProjectByTrnResponseDto?>.Failure(e.Message);
        }
    }
}