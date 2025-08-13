using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Entities;
using Microsoft.Extensions.Logging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Exception = System.Exception;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectGroupDetailsQuery(ProjectGroupId ProjectGroupId) : IRequest<Result<ProjectGroupDetails>>;

    public class GetProjectGroupDetailsQueryHandler(ICompleteRepository<ProjectGroup> projectGroupRepository,
        ICompleteRepository<Project> projectRepository,
        ITrustsV4Client trustsClient,
        IEstablishmentsV4Client establishmentsClient,
        ILogger<GetProjectGroupDetailsQueryHandler> logger)
        : IRequestHandler<GetProjectGroupDetailsQuery, Result<ProjectGroupDetails>>
    {
        public async Task<Result<ProjectGroupDetails>> Handle(GetProjectGroupDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectGroup = await projectGroupRepository
                    .Query()
                    .FirstOrDefaultAsync(pg => pg.Id == request.ProjectGroupId, cancellationToken);
                
                var trust = await trustsClient.GetTrustByUkprn2Async(projectGroup.TrustUkprn.ToString(), cancellationToken);
                
                var projectsInGroups = await projectRepository
                    .Query()
                        .Include(p => p.LocalAuthority)
                    .Where(p => p.GroupId == request.ProjectGroupId)
                    .ToListAsync(cancellationToken);
                
                var projectUrns = projectsInGroups.Select(p => p.Urn.Value).Distinct();
                var establishments = await establishmentsClient.GetByUrns2Async(projectUrns, cancellationToken);
                
                var cardDetails = projectsInGroups.Select(p =>
                {
                    var urn = p.Urn.Value.ToString();
                    var e = establishments.FirstOrDefault(e => e.Urn == urn);
                    var projectType = p.Type == ProjectType.Conversion ? "Conversion" : "Transfer";

                    return new ProjectGroupCardDetails(p.Id.Value.ToString(), e.Name, urn, projectType, p.LocalAuthority.Name, p.Region.ToDisplayDescription());
                });
                
                var result = new ProjectGroupDetails(projectGroup.Id.Value.ToString(), trust.Name, trust.ReferenceNumber, projectGroup.GroupIdentifier, cardDetails);

                return Result<ProjectGroupDetails>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectGroupDetailsQueryHandler), request);
                return PaginatedResult<ProjectGroupDetails>.Failure(ex.Message);
            }
        }
    }
}
