using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdatePrivateFinanceInitiativeTaskCommand(
        TaskDataId TaskDataId,
        bool? NotApplicable,
        bool? SupplementaryFundingAgreementPfiClausesInserted,
        bool? MasterFundingAgreementPfiClausesInserted,
        bool? Received,
        bool? DocumentsSentToSOPUForClearance,
        bool? Cleared,
        bool? DraftSaved,
        bool? SignedByAllStakeHolders,
        bool? FinalVersionSavedInSharepointFolder
    ) : IRequest<Result<bool>>;

    internal class UpdatePrivateFinanceInitiativeTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdatePrivateFinanceInitiativeTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdatePrivateFinanceInitiativeTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(
                p => p.Id == request.TaskDataId,
                cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");


            SetData(request, tasksData);

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }

        private static void SetData(UpdatePrivateFinanceInitiativeTaskCommand request, ConversionTasksData tasksData)
        {
            var notApplicableSelected = request.NotApplicable == true;

            tasksData.PrivateFinanceInitiativeNotApplicable = request.NotApplicable;

            if (notApplicableSelected)
            {
                tasksData.PrivateFinanceInitiativeSupplementaryFundingAgreementPfiClausesInserted = null;
                tasksData.PrivateFinanceInitiativeMasterFundingAgreementPfiClausesInserted = null;
                tasksData.PrivateFinanceInitiativeReceived = null;
                tasksData.PrivateFinanceInitiativeDocumentsSentToSOPUForClearance = null;
                tasksData.PrivateFinanceInitiativeCleared = null;
                tasksData.PrivateFinanceInitiativeDraftSavedInTrustSharepointFolder = null;
                tasksData.PrivateFinanceInitiativeSignedByAllStakeholders = null;
                tasksData.PrivateFinanceInitiativeFinalVersionSavedInSharepointFolder = null;
            }
            else
            {
                tasksData.PrivateFinanceInitiativeSupplementaryFundingAgreementPfiClausesInserted = request.SupplementaryFundingAgreementPfiClausesInserted;
                tasksData.PrivateFinanceInitiativeMasterFundingAgreementPfiClausesInserted = request.MasterFundingAgreementPfiClausesInserted;
                tasksData.PrivateFinanceInitiativeReceived = request.Received;
                tasksData.PrivateFinanceInitiativeDocumentsSentToSOPUForClearance = request.DocumentsSentToSOPUForClearance;
                tasksData.PrivateFinanceInitiativeCleared = request.Cleared;
                tasksData.PrivateFinanceInitiativeDraftSavedInTrustSharepointFolder = request.DraftSaved;
                tasksData.PrivateFinanceInitiativeSignedByAllStakeholders = request.SignedByAllStakeHolders;
                tasksData.PrivateFinanceInitiativeFinalVersionSavedInSharepointFolder = request.FinalVersionSavedInSharepointFolder;
            }
        }
    }
}