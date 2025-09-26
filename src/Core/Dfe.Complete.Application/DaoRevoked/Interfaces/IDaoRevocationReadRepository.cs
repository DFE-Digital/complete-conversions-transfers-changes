using Dfe.Complete.Domain.Entities;

namespace Dfe.Complete.Application.DaoRevoked.Interfaces
{
    public interface IDaoRevocationReadRepository
    {
        IQueryable<DaoRevocation> DaoRevocations { get; }
    }
}
