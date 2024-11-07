using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record NoteId(Guid Value) : IStronglyTypedId;

}
