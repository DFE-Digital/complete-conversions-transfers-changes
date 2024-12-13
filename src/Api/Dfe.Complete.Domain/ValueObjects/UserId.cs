using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record UserId(Guid Value) : IStronglyTypedId;

}
