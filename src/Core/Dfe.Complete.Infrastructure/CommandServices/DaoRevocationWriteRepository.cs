using Dfe.Complete.Application.DaoRevoked.Interfaces;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Infrastructure.Database;

namespace Dfe.Complete.Infrastructure.CommandServices
{
    internal class DaoRevocationWriteRepository(CompleteContext context) : IDaoRevocationWriteRepository
    {
        private readonly CompleteContext _context = context;

        public async Task CreateDaoRevocationAsync(DaoRevocation daoRevocation, CancellationToken cancellationToken)
        {
            await _context.DaoRevocations.AddAsync(daoRevocation, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateDaoRevocationReasonAsync(DaoRevocationReason daoRevocationReason, CancellationToken cancellationToken)
        {
            await _context.DaoRevocationReasons.AddAsync(daoRevocationReason, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
