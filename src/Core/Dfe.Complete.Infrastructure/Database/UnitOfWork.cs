using Dfe.Complete.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Dfe.Complete.Infrastructure.Database
{
    internal class UnitOfWork(CompleteContext context) : IUnitOfWork
    {
        private IDbContextTransaction? _transaction;

        public async Task BeingTransactionAsync()
        {
            _transaction = await context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await context.SaveChangesAsync();
                await context.Database.CommitTransactionAsync();
            }
        }

        public async Task RollBackAsync()
        {
            if (_transaction != null)
            {
                await context.Database.RollbackTransactionAsync();
            }
        }
    }
}
