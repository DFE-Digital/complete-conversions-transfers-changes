using Dfe.Complete.Application.Notes.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.CommandServices
{
    internal class TaskDataWriteRepository(CompleteContext context) : ITaskDataWriteRepository
    {
        public async Task UpdateConversionAsync(ConversionTasksData conversionTasksData, CancellationToken cancellationToken)
        {
            context.ConversionTasksData.Update(conversionTasksData);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateTransferAsync(TransferTasksData transferTasksData, CancellationToken cancellationToken)
        {
            context.TransferTasksData.Update(transferTasksData);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
