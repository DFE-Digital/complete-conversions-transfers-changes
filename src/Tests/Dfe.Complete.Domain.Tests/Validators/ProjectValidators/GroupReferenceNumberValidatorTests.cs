using Dfe.Complete.Domain.Validators.Project;
using FluentAssertions;

namespace Dfe.Complete.Domain.Tests.Validators.ProjectValidators
{
    public class GroupReferenceNumberValidatorTests
    {

        [Theory]
        [InlineData(null, true)]
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
            var result = new GroupReferenceNumberValidator().Validate(input);
            if (IsValid)
            {
                result.IsValid.Should().Be(true);
            }
            else
            {
                result.IsValid.Should().Be(false);
                result.Errors.Should().HaveCount(1);
                result.Errors[0].ErrorMessage.Should().Be("A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001");
            }
        }
    }
}
