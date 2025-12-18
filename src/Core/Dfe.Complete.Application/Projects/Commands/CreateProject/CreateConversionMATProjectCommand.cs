using Dfe.Complete.Application.Common.Interfaces;
using Dfe.Complete.Application.Projects.Services;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.Validators;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Application.Projects.Commands.CreateProject;

public record CreateConversionMatProjectCommand(
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
    string? AdvisoryBoardConditions,
    [GroupReferenceNumber] string? GroupId = null) : IRequest<ProjectId>;

public class CreateConversionMatProjectCommandHandler(
    IUnitOfWork unitOfWork,
    IHandoverProjectService handoverProjectService,
    ILogger<CreateConversionMatProjectCommandHandler> logger)
    : IRequestHandler<CreateConversionMatProjectCommand, ProjectId>
{
    public async Task<ProjectId> Handle(CreateConversionMatProjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();

            var urn = request.Urn!.Value;

            // Validate the request
            await handoverProjectService.ValidateUrnAsync(urn, cancellationToken);
            await handoverProjectService.ValidateTrnAndTrustNameAsync(
                request.NewTrustReferenceNumber!,
                request.NewTrustName!,
                cancellationToken);

            // Prepare common project data
            var commonData = await handoverProjectService.PrepareCommonProjectDataAsync(
                urn,
                request.CreatedByFirstName,
                request.CreatedByLastName,
                request.CreatedByEmail,
                cancellationToken);

            // Create conversion task data
            var conversionTask = handoverProjectService.CreateConversionTaskAsync();

            var parameters = new CreateConversionMatProjectParams(
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

            var project = Project.CreateConversionMATProject(parameters);

            project.PrepareId = request.PrepareId!.Value;

            await handoverProjectService.SaveProjectAndTaskAsync(project, conversionTask, cancellationToken);

            await unitOfWork.CommitAsync();

            return project.Id;
        }
        catch (Exception ex) when (ex is not UnprocessableContentException && ex is not NotFoundException && ex is not ValidationException)
        {
            await unitOfWork.RollBackAsync();
            logger.LogError(ex, "Exception while creating conversion project for URN: {Urn}", request.Urn);
            throw new UnknownException($"An error occurred while creating the conversion project for URN: {request.Urn}", ex);
        }
    }


}
