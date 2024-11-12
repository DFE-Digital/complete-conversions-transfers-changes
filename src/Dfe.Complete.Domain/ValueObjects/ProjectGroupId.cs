using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record ProjectGroupId(Guid Value) : IStronglyTypedId;
}
