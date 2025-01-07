using Dfe.Complete.Validators;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators
{
    public class GroupReferenceNumberAttributeTests
    {
        [Theory]
        [InlineData("", true)]
        [InlineData("abcd", false)]
        [InlineData("GRP_", false)]
        [InlineData("GRP_0", false)]
        [InlineData("GRP_0000", false)]
        [InlineData("GRP_12345678", true)]
        [InlineData("GRP_123456789", false)]
        [InlineData("GRP_000000001", false)]
        [InlineData("GRP_00000001", true)]
        [InlineData("GRP_0000001", false)]
        [InlineData("GRP_0000A001", false)]
        public void ValidateTests(string input, bool IsValid)
        {
            var result = new GroupReferenceNumberAttribute().GetValidationResult(input, new ValidationContext(new object()));
            if(IsValid)
            {
                result.Should().Be(ValidationResult.Success);
            }
            else
            {
                result.Should().NotBe(ValidationResult.Success);
                result?.ErrorMessage.Should().Be("A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001");
            }
        }
    }
}
