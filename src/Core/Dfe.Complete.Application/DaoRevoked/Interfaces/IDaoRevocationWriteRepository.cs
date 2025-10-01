using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.DaoRevoked.Interfaces
{
    public interface IDaoRevocationWriteRepository
    {
        Task CreateDaoRevocationAsync(DaoRevocation daoRevocation, CancellationToken cancellationToken);
        Task CreateDaoRevocationReasonAsync(DaoRevocationReason daoRevocationReason, CancellationToken cancellationToken);
    }
}
