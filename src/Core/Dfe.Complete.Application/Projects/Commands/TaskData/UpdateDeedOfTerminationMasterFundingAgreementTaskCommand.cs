using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateDeedOfTerminationMasterFundingAgreementTaskCommand(
        TaskDataId TaskDataId,
        bool? Received,
        bool? Cleared,
        bool? Saved,
        bool? Signed,
        bool? ContactFinancialReportingTeam,
        bool? SignedSecretaryState,
        bool? SavedAcademySharePointHolder,
        bool? NotApplicable) : IRequest<Result<bool>>;
    internal class UpdateDeedOfTerminationMasterFundingAgreementTaskCommandHandler(
       ITaskDataReadRepository taskDataReadRepository,
       ITaskDataWriteRepository taskDataWriteRepository)
       : IRequestHandler<UpdateDeedOfTerminationMasterFundingAgreementTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateDeedOfTerminationMasterFundingAgreementTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            var notApplicableSelected = request.NotApplicable == true;

            tasksData.DeedOfTerminationForTheMasterFundingAgreementReceived = notApplicableSelected ? null : request.Received;
            tasksData.DeedOfTerminationForTheMasterFundingAgreementCleared = notApplicableSelected ? null : request.Cleared;
            tasksData.DeedOfTerminationForTheMasterFundingAgreementSigned = notApplicableSelected ? null : request.Signed;
            tasksData.DeedOfTerminationForTheMasterFundingAgreementSavedAcademyAndOutgoingTrustSharepoint = notApplicableSelected ? null : request.Saved;
            tasksData.DeedOfTerminationForTheMasterFundingAgreementContactFinancialReportingTeam = notApplicableSelected ? null : request.ContactFinancialReportingTeam;
            tasksData.DeedOfTerminationForTheMasterFundingAgreementSignedSecretaryState = notApplicableSelected ? null : request.SignedSecretaryState;
            tasksData.DeedOfTerminationForTheMasterFundingAgreementSavedInAcademySharepointFolder = request.NotApplicable == true ? null : request.SavedAcademySharePointHolder;
            tasksData.DeedOfTerminationForTheMasterFundingAgreementNotApplicable = request.NotApplicable;

            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
