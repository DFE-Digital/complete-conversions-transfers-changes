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

                var trustUkprn = projectGroup?.TrustUkprn?.ToString();
                
                var trust = await trustsClient.GetTrustByUkprn2Async(trustUkprn!, cancellationToken);
                
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
                    var establishmentName = establishments.FirstOrDefault(e => e.Urn == urn)?.Name ?? string.Empty;
                    var projectType = p.Type == ProjectType.Conversion ? "Conversion" : "Transfer";

                    return new ProjectGroupCardDetails(p.Id.Value.ToString(), establishmentName, urn, projectType, p.LocalAuthority!.Name, p.Region.ToDisplayDescription());
                });
                
                
                string trustName = trust.Name ?? string.Empty;
                string trustReference = trust.ReferenceNumber ?? string.Empty;
                string groupIdentifier = projectGroup?.GroupIdentifier ?? string.Empty;
                string id = projectGroup?.Id.ToString() ?? string.Empty;
                
                var result = new ProjectGroupDetails(id, trustName, trustReference, groupIdentifier, cardDetails);

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
