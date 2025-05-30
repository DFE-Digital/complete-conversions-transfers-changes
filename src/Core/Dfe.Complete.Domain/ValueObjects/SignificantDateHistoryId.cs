using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record SignificantDateHistoryId(Guid Value) : IStronglyTypedId;
}
