using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record GiasGroupId(Guid Value) : IStronglyTypedId;
}
