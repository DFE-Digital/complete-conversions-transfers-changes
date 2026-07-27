using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Application.Projects.Interfaces;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Utils.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dfe.Complete.Application.Projects.Commands.TaskData
{
    public record UpdateLandQuestionnaireTaskCommand(
        TaskDataId TaskDataId,
        bool? LandQuestionnaireReceived,
        bool? LandQuestionnaireCleared,
        bool? LandQuestionnaireSigned,
        bool? LandQuestionnaireSaved,
        bool? LandRegistryTitlePlansReceived,
        bool? LandRegistryTitlePlansCleared,
        bool? LandRegistryTitlePlansSaved
    ) : IRequest<Result<bool>>;

    internal class UpdateLandQuestionnaireTaskCommandHandler(
        ITaskDataReadRepository taskDataReadRepository,
        ITaskDataWriteRepository taskDataWriteRepository)
        : IRequestHandler<UpdateLandQuestionnaireTaskCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(UpdateLandQuestionnaireTaskCommand request, CancellationToken cancellationToken)
        {
            var tasksData = await taskDataReadRepository.ConversionTaskData.FirstOrDefaultAsync(p => p.Id == request.TaskDataId, cancellationToken)
                ?? throw new NotFoundException($"Conversion task data {request.TaskDataId} not found.");

            tasksData.LandQuestionnaireReceived = request.LandQuestionnaireReceived;
            tasksData.LandQuestionnaireCleared = request.LandQuestionnaireCleared;
            tasksData.LandQuestionnaireSigned = request.LandQuestionnaireSigned;
            tasksData.LandQuestionnaireSaved = request.LandQuestionnaireSaved;
            tasksData.LandRegistryReceived = request.LandRegistryTitlePlansReceived;
            tasksData.LandRegistryCleared = request.LandRegistryTitlePlansCleared;
            tasksData.LandRegistrySaved = request.LandRegistryTitlePlansSaved;

            await taskDataWriteRepository.UpdateConversionAsync(tasksData, DateTime.UtcNow, cancellationToken);

            return Result<bool>.Success(true);
        }
    }
}
