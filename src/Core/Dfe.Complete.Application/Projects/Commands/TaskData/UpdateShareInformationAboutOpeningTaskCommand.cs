using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore; 

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateShareInformationAboutOpeningTaskCommand(
        TaskDataId TaskDataId, 
        bool? ShareInformationEmail
    ) : IRequest<Result<bool>>;

    internal class UpdateShareInformationAboutOpeningTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateShareInformationAboutOpeningTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateShareInformationAboutOpeningTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.ShareInformationEmail = request.ShareInformationEmail;           

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        } 
    }
}
