using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.Projects.Interfaces
{
    public interface ITaskDataReadRepository
    {
        IQueryable<ConversionTasksData> ConversionTaskData { get; }
        IQueryable<TransferTasksData> TransferTaskData { get; }
    }
}
