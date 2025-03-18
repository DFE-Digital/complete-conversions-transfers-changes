namespace Dfe.Complete.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollBackAsync();
    }
}
