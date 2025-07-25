using Dfe.Complete.Domain.Entities; 

namespace Dfe.Complete.Application.Notes.Interfaces
{
    public interface ITaskDataWriteRepository
    {
        Task UpdateConversionAsync(ConversionTasksData conversionTasksData, CancellationToken cancellationToken);
        Task UpdateTransferAsync(TransferTasksData transferTasksData, CancellationToken cancellationToken);
    }
}
