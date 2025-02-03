using System.Security.Claims;
using Dfe.Complete.Extensions;

namespace Dfe.Complete.Tests.Extensions
{
    public class UserExtensionsTests
    {
        [Fact]
        public void GetUserAdId_Returns_CorrectValue_When_ObjectIdentifier_Claim_Exists()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("objectidentifier", "12345-abcde")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = principal.GetUserAdId();

            // Assert
            Assert.Equal("12345-abcde", result);
        }

        [Fact]
        public void GetUserAdId_Returns_Null_When_ObjectIdentifier_Claim_Does_Not_Exist()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("someotherclaim", "value")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = principal.GetUserAdId();

            // Assert
            Assert.Null(result);
        }
    }
}
