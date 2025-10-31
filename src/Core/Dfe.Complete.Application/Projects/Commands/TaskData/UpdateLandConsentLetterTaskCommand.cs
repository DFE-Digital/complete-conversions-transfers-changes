using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{   public record UpdateLandConsentLetterTaskCommand(
        TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? Drafted,
        bool? Signed,
        bool? Sent,
        bool? Saved
    ) : IRequest<Result<bool>>;

    internal class UpdateLandConsentLetterTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateLandConsentLetterTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateLandConsentLetterTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            var notApplicableSelected = request.NotApplicable == true;

            tasksData.LandConsentLetterNotApplicable = request.NotApplicable;
            tasksData.LandConsentLetterDrafted = notApplicableSelected ? null : request.Drafted;
            tasksData.LandConsentLetterSigned = notApplicableSelected ? null : request.Signed; 
            tasksData.LandConsentLetterSent = notApplicableSelected ? null : request.Sent;
            tasksData.LandConsentLetterSaved = notApplicableSelected ? null : request.Saved;
            

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
