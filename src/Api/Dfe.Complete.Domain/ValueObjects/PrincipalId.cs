using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record PrincipalId(int Value) : IStronglyTypedId;
}
