namespace Dfe.Complete.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        Task BeingTransactionAsync();
        Task CommitAsync();
        Task RollBackAsync();
    }
}
