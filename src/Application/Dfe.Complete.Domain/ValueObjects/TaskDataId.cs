using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record TaskDataId(Guid Value) : IStronglyTypedId;

}
