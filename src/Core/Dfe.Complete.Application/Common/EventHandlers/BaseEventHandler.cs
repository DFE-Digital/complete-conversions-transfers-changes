using Dfe.Complete.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Dfe.Complete.Application.Common.EventHandlers
{
#pragma warning disable S2629, S2139
    public abstract class BaseEventHandler<TEvent>(ILogger<BaseEventHandler<TEvent>> logger)
        : INotificationHandler<TEvent>
        where TEvent : IDomainEvent
    {
        public virtual async Task Handle(TEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                logger.LogInformation("Handling event: {EventName}", typeof(TEvent).Name);

                await HandleEvent(notification, cancellationToken);

                logger.LogInformation("Event handled successfully: {EventName}", typeof(TEvent).Name);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error handling event: {EventName}", typeof(TEvent).Name);
                throw;
            }
        }

        protected virtual Task HandleEvent(TEvent notification, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
#pragma warning restore S2629, S2139
}
