using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.ProjectGroups.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Exception = System.Exception;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{
    public record GetProjectGroupDetailsQuery(ProjectGroupId ProjectGroupId) : IRequest<Result<ProjectGroupDetails>>;

    public class GetProjectGroupDetailsQueryHandler(IProjectGroupReadRepository projectGroupRepository,
        IProjectReadRepository projectReadRepository,
        ITrustsV4Client trustsClient,
        IEstablishmentsV4Client establishmentsClient,
        ILogger<GetProjectGroupDetailsQueryHandler> logger)
        : IRequestHandler<GetProjectGroupDetailsQuery, Result<ProjectGroupDetails>>
    {
        public async Task<Result<ProjectGroupDetails>> Handle(GetProjectGroupDetailsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var projectGroup = await projectGroupRepository.ProjectGroups.AsNoTracking()
                    .FirstOrDefaultAsync(pg => pg.Id == request.ProjectGroupId, cancellationToken);

                var trustUkprn = projectGroup?.TrustUkprn?.ToString();

                string trustName = "Could Not Find Trust For Ukprn";
                string trustReference = string.Empty;

                if (!string.IsNullOrEmpty(trustUkprn))
                {
                    var trust = await trustsClient.GetTrustByUkprn2Async(trustUkprn!, cancellationToken);

                    trustName = trust.Name!;
                    trustReference = trust.ReferenceNumber!;
                }

                var projectsInGroups = await projectReadRepository.ProjectsNoIncludes
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


                string groupIdentifier = projectGroup?.GroupIdentifier ?? string.Empty;
                string id = projectGroup?.Id.Value.ToString() ?? string.Empty;

                var result = new ProjectGroupDetails(id, trustName, trustReference, groupIdentifier, cardDetails.OrderBy(c => c.EstablishmentName));

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
