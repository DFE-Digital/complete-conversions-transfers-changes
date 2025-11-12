using Dfe.Complete.Application.Services.BackgroundServices.Events;
using GovUK.Dfe.CoreLibs.AsyncProcessing.Interfaces;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Services.BackgroundServices.EventHandlers
{
    public class SimpleTaskCompletedEventHandler(ILogger<SimpleTaskCompletedEventHandler> logger) : IBackgroundServiceEventHandler<CreateReportExampleTaskCompletedEvent>
    {
        public Task Handle(CreateReportExampleTaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Event received for Task: {TaskName}, Message: {Message}", notification.TaskName, notification.Message);
            return Task.CompletedTask;
        }
    }
}
