using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateSubleasesTaskCommand(
        TaskDataId TaskDataId, 
        bool? NotApplicable,
        bool? Received,
        bool? Cleared,
        bool? Signed, 
        bool? Saved,
        bool? EmailSigned,
        bool? SaveSigned,
        bool? ReceiveSigned
    ) : IRequest<Result<bool>>;

    internal class UpdateSubleasesTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateSubleasesTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateSubleasesTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.SubleasesNotApplicable = request.NotApplicable;
            tasksData.SubleasesReceived = request.NotApplicable == true ? null : request.Received;
            tasksData.SubleasesCleared = request.NotApplicable == true ? null : request.Cleared;
            tasksData.SubleasesSigned = request.NotApplicable == true ? null : request.Signed;
            tasksData.SubleasesSaved = request.NotApplicable == true ? null : request.Saved;
            tasksData.SubleasesEmailSigned = request.NotApplicable == true ? null : request.EmailSigned;
            tasksData.SubleasesSaveSigned = request.NotApplicable == true ? null : request.SaveSigned;
            tasksData.SubleasesReceiveSigned = request.NotApplicable == true ? null : request.ReceiveSigned;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }  
    }
}
