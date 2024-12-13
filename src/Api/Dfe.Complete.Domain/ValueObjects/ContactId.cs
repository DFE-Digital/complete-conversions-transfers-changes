using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record ContactId(Guid Value) : IStronglyTypedId;

}
