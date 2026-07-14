using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Utils.Exceptions;
using static Dfe.Complete.Utils.Helpers.NotApplicableHelper;
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

            var notApplicable = request.ChildrenCentreNotApplicable;
            tasksData.ChildrenCentreNotApplicable = notApplicable;
            
            tasksData.ChildrenCentreLocalAuthority = NullWhenNotApplicable(notApplicable, request.ChildrenCentreLocalAuthority);
            tasksData.ChildrenCentreAcademyTrust = NullWhenNotApplicable(notApplicable, request.ChildrenCentreAcademyTrust);
            tasksData.ChildrenCentreStaffingAndTransferReviewed = NullWhenNotApplicable(notApplicable, request.ChildrenCentreStaffingAndTransferReviewed);
            tasksData.ChildrenCentreLandLeaseSharedAgreed = NullWhenNotApplicable(notApplicable, request.ChildrenCentreLandLeaseSharedAgreed);
            tasksData.ChildrenCentreFundingPensionReviewed = NullWhenNotApplicable(notApplicable, request.ChildrenCentreFundingPensionReviewed);
            tasksData.ChildrenCentreLegalAndGovernanceReviewed = NullWhenNotApplicable(notApplicable, request.ChildrenCentreLegalAndGovernanceReviewed);

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
