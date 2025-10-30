using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.CreateHandoverProject;

public record CreateHandoverConversionMatProjectCommand(
    [Required][Urn] int? Urn,
    [Required][Trn] string? NewTrustReferenceNumber,
    [Required] string? NewTrustName,
    [Required][PastDate(AllowToday = true)] DateOnly? AdvisoryBoardDate,
    [Required][FirstOfMonthDate] DateOnly? ProvisionalConversionDate,
    [Required][InternalEmail] string CreatedByEmail,
    [Required] string CreatedByFirstName,
    [Required] string CreatedByLastName,
    [Required] int? PrepareId,
    [Required] bool? DirectiveAcademyOrder,
    string? AdvisoryBoardConditions) : IRequest<ProjectId>;

public class CreateHandoverConversionMatProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateHandoverConversionMatProjectCommandHandler> logger)
    : IRequestHandler<CreateHandoverConversionMatProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateHandoverConversionMatProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var urn = request.Urn!.Value;

            // Validate the request
            await handoverProjectService.ValidateUrnAsync(urn, cancellationToken);

            // Prepare common project data
            var commonData = await handoverProjectService.PrepareCommonProjectDataAsync(
                urn,
                request.CreatedByFirstName,
                request.CreatedByLastName,
                request.CreatedByEmail,
                cancellationToken);

            // Create conversion task data
            var conversionTask = handoverProjectService.CreateConversionTaskAsync();

            var parameters = new CreateHandoverConversionMatProjectParams(
                commonData.ProjectId,
                new Urn(commonData.Urn),
                conversionTask.Id.Value,
                request.ProvisionalConversionDate!.Value,
                commonData.Region,
                request.DirectiveAcademyOrder ?? false,
                request.AdvisoryBoardDate!.Value,
                request.AdvisoryBoardConditions ?? null,
                commonData.UserId,
                commonData.LocalAuthorityId,
                request.NewTrustReferenceNumber!,
                request.NewTrustName!
                );

            var project = Project.CreateHandoverConversionMATProject(parameters);

            project.PrepareId = request.PrepareId!.Value;

            await handoverProjectService.SaveProjectAndTaskAsync(project, conversionTask, cancellationToken);

            await unitOfWork.CommitAsync();

            return project.Id;
        }
        catch (Exception ex) when (ex is not UnprocessableContentException && ex is not NotFoundException && ex is not ValidationException)
        {
            await unitOfWork.RollBackAsync();
            logger.LogError(ex, "Exception while creating handover conversion project for URN: {Urn}", request.Urn);
            throw new UnknownException($"An error occurred while creating the handover conversion project for URN: {request.Urn}", ex);
        }
    }


}
