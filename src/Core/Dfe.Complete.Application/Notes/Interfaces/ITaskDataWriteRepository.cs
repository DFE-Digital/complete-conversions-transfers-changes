using Dfe.Complete.Domain.Entities; 

namespace Dfe.Complete.Application.Notes.Interfaces
{
    public interface ITaskDataWriteRepository
    {
        Task UpdateConversionAsync(ConversionTasksData conversionTasksData, DateTime dateTime, CancellationToken cancellationToken);
        Task UpdateTransferAsync(TransferTasksData transferTasksData, DateTime dateTime, CancellationToken cancellationToken);
    }
}
