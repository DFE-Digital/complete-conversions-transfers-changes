using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record ProjectId(Guid Value) : IStronglyTypedId, IComparable<ProjectId>
    {
        public int CompareTo(ProjectId? other)
        {
            return other == null ? 1 : Value.CompareTo(other.Value);
        }
    }
}
