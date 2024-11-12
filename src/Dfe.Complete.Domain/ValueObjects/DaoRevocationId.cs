using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record DaoRevocationId(Guid Value) : IStronglyTypedId;

}
