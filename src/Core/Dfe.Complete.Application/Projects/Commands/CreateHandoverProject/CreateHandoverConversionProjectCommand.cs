using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Utils;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Domain.Validators;
using Microsoft.Extensions.Logging;
using Dfe.Complete.Application.Projects.Services;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

public record CreateHandoverConversionProjectCommand(
    [Required]
    [Urn]
    int? Urn,
    [Required]
    [Ukprn]
    int? IncomingTrustUkprn,
    [Required]
    [PastDate (AllowToday = true)]
    DateOnly? AdvisoryBoardDate,
    [Required]
    [FirstOfMonthDate]
    DateOnly? ProvisionalConversionDate,
    [Required]
    [InternalEmail]
    string CreatedByEmail,
    [Required] string CreatedByFirstName,
    [Required] string CreatedByLastName,
    [Required] int? PrepareId,
    [Required] bool? DirectiveAcademyOrder,
    string? AdvisoryBoardConditions,
    [GroupReferenceNumber]
    string? GroupId = null) : IRequest<ProjectId>;

public class CreateHandoverConversionProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateHandoverConversionProjectCommandHandler> logger)
    : IRequestHandler<CreateHandoverConversionProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverConversionProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var urn = request.Urn!.Value;
            var incomingTrustUkprn = request.IncomingTrustUkprn!.Value;

            // Validate the request
            await handoverProjectService.ValidateUrnAndTrustsAsync(urn, incomingTrustUkprn, cancellationToken: cancellationToken);

            // Prepare common project data
            var commonData = await handoverProjectService.PrepareCommonProjectDataAsync(
                urn, 
                incomingTrustUkprn, 
                request.GroupId, 
                request.CreatedByFirstName, 
                request.CreatedByLastName, 
                request.CreatedByEmail, 
                cancellationToken);

            // Create conversion task data
            var conversionTask = handoverProjectService.CreateConversionTaskAsync();

            var parameters = new CreateHandoverConversionProjectParams(
                commonData.ProjectId,
                new Urn(commonData.Urn),
                conversionTask.Id.Value,
                request.ProvisionalConversionDate!.Value,
                new Ukprn(incomingTrustUkprn),
                commonData.Region,
                request.DirectiveAcademyOrder ?? false,
                request.AdvisoryBoardDate!.Value,
                request.AdvisoryBoardConditions ?? null,
                commonData.GroupId,
                commonData.UserId,
                commonData.LocalAuthorityId);

            var project = Project.CreateHandoverConversionProject(parameters);

            project.PrepareId = request.PrepareId!.Value;

            await handoverProjectService.SaveProjectAndTaskAsync(project, conversionTask, cancellationToken);

            await unitOfWork.CommitAsync();

            return project.Id;
        }
        catch (Exception ex) when (ex is not NotFoundException && ex is not ValidationException)
        {
            await unitOfWork.RollBackAsync();
            logger.LogError(ex, "Exception while creating handover conversion project for URN: {Urn}", request.Urn);
            throw new UnknownException($"An error occurred while creating the handover conversion project for URN: {request.Urn}", ex);
        }
    }


}
