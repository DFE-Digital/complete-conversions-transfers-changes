using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateTenancyAtWillTaskCommand(
        TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? EmailSigned,
        bool? SaveSigned,
        bool? ReceiveSigned
    ) : IRequest<Result<bool>>;

    internal class UpdateTenancyAtWillTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateTenancyAtWillTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateTenancyAtWillTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.TenancyAtWillNotApplicable = request.NotApplicable;
            tasksData.TenancyAtWillEmailSigned = request.NotApplicable == true ? null : request.EmailSigned;
            tasksData.TenancyAtWillSaveSigned = request.NotApplicable == true ? null : request.SaveSigned;
            tasksData.TenancyAtWillReceiveSigned = request.NotApplicable == true ? null : request.ReceiveSigned;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}