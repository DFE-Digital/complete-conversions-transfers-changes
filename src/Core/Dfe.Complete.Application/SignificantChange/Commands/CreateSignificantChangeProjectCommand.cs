using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Interfaces.Repositories;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.SignificantChange.Commands;

public record CreateSignificantChangeProjectCommand(
    [Required][Urn] int? AcademyUrn,
    [Required][Ukprn(ValueIsInteger = true)] int? TrustUkprn,
    [Required] string? TrustName,
    [Required] int? PrepareId,
    [Required][InternalEmail] string? DecisionRecordedByEmail,
    [Required] string? DecisionRecordedByFirstName,
    [Required] string? DecisionRecordedByLastName,
    string? DecisionConditions
) : IRequest<ProjectId>;

public class CreateSignificantChangeProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ICompleteRepository<SignificantChangeProject> significantChangeProjectRepository,
    ILogger<CreateSignificantChangeProjectCommandHandler> logger)
    : IRequestHandler<CreateSignificantChangeProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateSignificantChangeProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var urn = request.AcademyUrn!.Value;
            var trustUkprn = request.TrustUkprn!.Value;

            // Validate the request
            await handoverProjectService.ValidateTrustAsync(trustUkprn, cancellationToken: cancellationToken);
            
            var commonData = await handoverProjectService.PrepareCommonProjectDataAsync(
                urn,
                request.DecisionRecordedByFirstName!,
                request.DecisionRecordedByLastName!,
                request.DecisionRecordedByEmail!,
                cancellationToken);

            var project = SignificantChangeProject.CreateProject(
                new Ukprn(trustUkprn),
                request.TrustName!,
                new Urn(urn));

            project.PrepareId = request.PrepareId!.Value;
            project.Region = commonData.Region;
            project.DecisionConditions = request.DecisionConditions;
            project.LocalAuthorityId = new LocalAuthorityId(commonData.LocalAuthorityId);

            project.AssignUser(commonData.UserId);

            await significantChangeProjectRepository.AddAsync(project, cancellationToken);
            await unitOfWork.CommitAsync();

            return project.Id;
        }
        catch (Exception ex) when (ex is not UnprocessableContentException && ex is not NotFoundException && ex is not ValidationException)
        {
            await unitOfWork.RollBackAsync();
            logger.LogError(ex, "Exception while creating significant change project for URN: {Urn}", request.AcademyUrn);
            throw new UnknownException($"An error occurred while creating the significant change project for URN: {request.AcademyUrn}", ex);
        }
    }
}
