using Dfe.Complete.Domain.Common;

namespace Dfe.Complete.Domain.ValueObjects
{
    public record Ukprn(int Value) : IStronglyTypedId
    {
        public static implicit operator Ukprn(string Value)
        {
            return string.IsNullOrWhiteSpace(Value) ? null : new Ukprn(int.Parse(Value));
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }

}
