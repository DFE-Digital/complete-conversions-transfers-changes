using MediatR;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Utils;
using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Domain.Validators;
using Microsoft.Extensions.Logging;
using Dfe.Complete.Application.Projects.Services;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

public record CreateHandoverConversionMATProjectCommand(
    [Required]
    [Urn]
    int? Urn,
    [Required]
    [Trn]
    int? NewTrustReferenceNumber,
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

public class CreateHandoverConversionMATProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateHandoverConversionMATProjectCommandHandler> logger)
    : IRequestHandler<CreateHandoverConversionMATProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverConversionMATProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var urn = request.Urn!.Value;

            // Validate the request
            await handoverProjectService.ValidateUrnAsync(urn, cancellationToken: cancellationToken);

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

            var parameters = new CreateHandoverConversionMATProjectParams(
                commonData.ProjectId,
                new Urn(commonData.Urn),
                conversionTask.Id.Value,
                request.ProvisionalConversionDate!.Value,
                commonData.Region,
                request.DirectiveAcademyOrder ?? false,
                request.AdvisoryBoardDate!.Value,
                request.AdvisoryBoardConditions ?? null,
                commonData.GroupId,
                commonData.UserId,
                commonData.LocalAuthorityId,
                request.NewTrustReferenceNumber,
                request.NewTrustReferenceName
                );

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
