using DfE.CoreLibs.AsyncProcessing.Interfaces;
using Dfe.Complete.Application.Services.BackgroundServices.Events;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Services.BackgroundServices.EventHandlers
{
    public class SimpleTaskCompletedEventHandler(ILogger<SimpleTaskCompletedEventHandler> logger) : IBackgroundServiceEventHandler<CreateReportExampleTaskCompletedEvent>
    {
        public Task Handle(CreateReportExampleTaskCompletedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Event received for Task: {notification.TaskName}, Message: {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
