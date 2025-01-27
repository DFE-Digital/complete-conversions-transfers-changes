using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using MediatR;

namespace Dfe.Complete.Application.Projects.Queries.GetProject;


public record GetProjectByTrnQuery(string NewTrustReferenceNumber) : IRequest<Result<Project?>>;

public class GetProjectByTrn(ICompleteRepository<Project> projectRepo) : IRequestHandler<GetProjectByTrnQuery, Result<Project?>>
{
    public async Task<Result<Project?>> Handle(GetProjectByTrnQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await projectRepo.FindAsync(x => x.NewTrustReferenceNumber == request.NewTrustReferenceNumber, cancellationToken);
            return Result<Project?>.Success(result);
        }
        catch (Exception e)
        {
            return Result<Project?>.Failure(e.Message);
        }
    }
}