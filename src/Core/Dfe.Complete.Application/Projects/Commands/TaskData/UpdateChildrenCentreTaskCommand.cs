using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Utils.Exceptions;
using Microsoft.EntityFrameworkCore;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;


namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateChildrenCentreTaskCommand(
        TaskDataId TaskDataId,
        bool? ChildrenCentreNotApplicable,
        bool? ChildrenCentreLocalAuthority,
        bool? ChildrenCentreAcademyTrust,
        bool? ChildrenCentreStaffingAndTransferReviewed,
        bool? ChildrenCentreLandLeaseSharedAgreed,
        bool? ChildrenCentreFundingPensionReviewed,
        bool? ChildrenCentreLegalAndGovernanceReviewed
    ) : IRequest<Result<bool>>;

    internal class UpdateChildrenCentreTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateChildrenCentreTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateChildrenCentreTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                            ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");
            
            tasksData.ChildrenCentreNotApplicable = request.ChildrenCentreNotApplicable;
            tasksData.ChildrenCentreLocalAuthority = request.ChildrenCentreLocalAuthority;
            tasksData.ChildrenCentreAcademyTrust = request.ChildrenCentreAcademyTrust;
            tasksData.ChildrenCentreStaffingAndTransferReviewed = request.ChildrenCentreStaffingAndTransferReviewed;
            tasksData.ChildrenCentreLandLeaseSharedAgreed = request.ChildrenCentreLandLeaseSharedAgreed;
            tasksData.ChildrenCentreFundingPensionReviewed = request.ChildrenCentreFundingPensionReviewed;
            tasksData.ChildrenCentreLegalAndGovernanceReviewed = request.ChildrenCentreLegalAndGovernanceReviewed;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
