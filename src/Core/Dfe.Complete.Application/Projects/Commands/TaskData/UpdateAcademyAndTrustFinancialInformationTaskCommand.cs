using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore; 
using System.ComponentModel.DataAnnotations; 

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{ 
    public record UpdateAcademyAndTrustFinancialInformationTaskCommand(
        [Required] TaskDataId TaskDataId, 
        bool? NotApplicable,
        string? AcademySurplusOrDeficit,
        string? TrustSurplusOrDeficit
    ) : IRequest<Result<bool>>;

    internal class UpdateAcademyAndTrustFinancialInformationTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateAcademyAndTrustFinancialInformationTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateAcademyAndTrustFinancialInformationTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.TransferTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Transfer task data {request.TaskDataId} not found.");

            tasksData.CheckAndConfirmFinancialInformationNotApplicable = request.NotApplicable; 
            tasksData.CheckAndConfirmFinancialInformationAcademySurplusDeficit = request.NotApplicable == true ? null : request.AcademySurplusOrDeficit;
            tasksData.CheckAndConfirmFinancialInformationTrustSurplusDeficit = request.NotApplicable == true ? null : request.TrustSurplusOrDeficit;
            
            await taskDataWriteRepository.UpdateTransferAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }  
    }
}
