using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateProcessSupportGrantTaskCommand(
        TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? ConversionGrantCheckVendorAccount,
        bool? ConversionGrantPaymentForm,
        bool? ConversionGrantSendInformation,
        bool? ConversionGrantSharePaymentDate
    ) : IRequest<Result<bool>>;

    internal class UpdateProcessSupportGrantTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateProcessSupportGrantTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateProcessSupportGrantTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.ConversionGrantNotApplicable = request.NotApplicable;
            tasksData.ConversionGrantCheckVendorAccount = request.NotApplicable == true ? null : request.ConversionGrantCheckVendorAccount;
            tasksData.ConversionGrantPaymentForm = request.NotApplicable == true ? null : request.ConversionGrantPaymentForm;
            tasksData.ConversionGrantSendInformation = request.NotApplicable == true ? null : request.ConversionGrantSendInformation;
            tasksData.ConversionGrantSharePaymentDate = request.NotApplicable == true ? null : request.ConversionGrantSharePaymentDate;          

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
