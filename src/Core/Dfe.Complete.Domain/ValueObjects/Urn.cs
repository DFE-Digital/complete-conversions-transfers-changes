using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record Urn(int Value) : IStronglyTypedId;

}
