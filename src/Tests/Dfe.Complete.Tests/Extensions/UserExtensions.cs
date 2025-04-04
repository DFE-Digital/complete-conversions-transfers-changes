using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Extensions;
using Dfe.Complete.Utils;
using MediatR;
using Moq;
using Xunit;

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

        [Fact]
        public async Task GetUser_Returns_UserDto_When_UserExists()
        {
            // Arrange
            var userAdId = "12345-abcde";
            var claims = new List<Claim> { new Claim("objectidentifier", userAdId) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var expectedUser = new UserDto { ActiveDirectoryUserId = userAdId };

            var senderMock = new Mock<ISender>();
            senderMock.Setup(s => s.Send(It.Is<GetUserByAdIdQuery>(q => q.UserAdId == userAdId), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(Result<UserDto>.Success(expectedUser));

            // Act
            var result = await principal.GetUser(senderMock.Object);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.ActiveDirectoryUserId, result.ActiveDirectoryUserId);
        }

        [Fact]
        public async Task GetUser_Throws_NotFoundException_When_User_Not_Found()
        {
            // Arrange
            var userAdId = "12345-abcde";
            var claims = new List<Claim> { new Claim("objectidentifier", userAdId) };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var senderMock = new Mock<ISender>();
            senderMock.Setup(s => s.Send(It.Is<GetUserByAdIdQuery>(q => q.UserAdId == userAdId), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(Result<UserDto>.Failure("User not found"));

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => principal.GetUser(senderMock.Object));
        }

        [Fact]
        public async Task GetUser_Throws_Exception_When_Claim_Is_Missing()
        {
            // Arrange
            var claims = new List<Claim> { new Claim("someotherclaim", "value") };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var senderMock = new Mock<ISender>();

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => principal.GetUser(senderMock.Object));
        }
    }
}
