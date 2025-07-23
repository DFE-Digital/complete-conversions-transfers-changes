using Dfe.AcademiesApi.Client.Contracts;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.QueryFilters;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace Dfe.Complete.Application.Projects.Queries.GetProject
{ 
    public record GetProjectWithEstablishmentByIdQuery(
        ProjectId ProjectId) : IRequest<Result<ProjectWithEstablishmentQueryModel>>;

    public class GetProjectWithEstablishmentByIdQueryHandler(
        IProjectReadRepository projectReadRepository,
        ITrustsV4Client trustsClient,
        ILogger<GetProjectWithEstablishmentByIdQueryHandler> logger)
        : IRequestHandler<GetProjectWithEstablishmentByIdQuery, Result<ProjectWithEstablishmentQueryModel>>
    {
        public async Task<Result<ProjectWithEstablishmentQueryModel>> Handle(GetProjectWithEstablishmentByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var project = await new ProjectIdQuery(request.ProjectId)
                   .Apply(projectReadRepository.Projects.AsNoTracking())
                   .Include(p => p.GiasEstablishment)
                   .FirstOrDefaultAsync(cancellationToken) ?? throw new NotFoundException($"Project {request.ProjectId} not found", nameof(request.ProjectId));

                var allProjectTrustUkPrns = new[]
                {
                    project?.IncomingTrustUkprn?.Value.ToString() ?? string.Empty,
                   project?.OutgoingTrustUkprn?.Value.ToString() ?? string.Empty
                }; 

                var allTrusts = await GetTrustsByUkprns(allProjectTrustUkPrns, cancellationToken);

                var incomingTrustName = project!.FormAMat ? project.NewTrustName : allTrusts?.FirstOrDefault(trust => trust.Ukprn == project?.IncomingTrustUkprn?.Value.ToString())?.Name;
                var outgoingTrustName = allTrusts?.FirstOrDefault(trust => trust.Ukprn == project?.OutgoingTrustUkprn?.Value.ToString())?.Name;
                var result = ProjectWithEstablishmentQueryModel.MapProjectAndEstablishmentToModel(project, project.GiasEstablishment!, incomingTrustName!, outgoingTrustName);
                return Result<ProjectWithEstablishmentQueryModel>.Success(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception for {Name} Request - {@Request}", nameof(GetProjectWithEstablishmentByIdQueryHandler), request);
                return Result<ProjectWithEstablishmentQueryModel>.Failure(ex.Message);
            }
        }
        private async Task<ObservableCollection<TrustDto>> GetTrustsByUkprns(string[] allProjectTrustUkPrns, CancellationToken cancellationToken)
           => allProjectTrustUkPrns.All(string.IsNullOrEmpty) ?
               [] : await trustsClient.GetByUkprnsAllAsync(allProjectTrustUkPrns, cancellationToken);
    }
}
