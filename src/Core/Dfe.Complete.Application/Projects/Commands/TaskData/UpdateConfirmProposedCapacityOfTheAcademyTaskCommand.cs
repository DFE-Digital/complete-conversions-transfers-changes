using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateConfirmProposedCapacityOfTheAcademyTaskCommand(TaskDataId TaskDataId,
        bool? NotApplicable,
        string? ReceptionToSixYears,
        string? SevenToElevenYears,
        string? TwelveOrAboveYears) : IRequest<Result<bool>>;

    public class UpdateConfirmProposedCapacityOfTheAcademyTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateConfirmProposedCapacityOfTheAcademyTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateConfirmProposedCapacityOfTheAcademyTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.ProposedCapacityOfTheAcademyNotApplicable = request.NotApplicable; 
            tasksData.ProposedCapacityOfTheAcademyReceptionToSixYears = request.NotApplicable == true ? null : request.ReceptionToSixYears;
            tasksData.ProposedCapacityOfTheAcademySevenToElevenYears = request.NotApplicable == true ? null : request.SevenToElevenYears;
            tasksData.ProposedCapacityOfTheAcademyTwelveOrAboveYears = request.NotApplicable == true ? null : request.TwelveOrAboveYears; 

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        } 
    }
}
