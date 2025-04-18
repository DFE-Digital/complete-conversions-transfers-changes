﻿using System.Security.Claims;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetUser;
using Dfe.Complete.Domain.Enums;
using Dfe.Complete.Extensions;
using MediatR;
using Moq;

namespace Dfe.Complete.Tests.Extensions
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void GetUserAdId_Returns_CorrectValue_When_ObjectIdentifier_Claim_Exists()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new ("objectidentifier", "12345-abcde")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = principal.GetUserAdId();

            // Assert
            Assert.Equal("12345-abcde", result);
        }

        [Fact]
        public void GetUserAdId_Returns_Throws_When_ObjectIdentifier_Claim_Does_Not_Exist()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new ("someotherclaim", "value")
            };
            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                principal.GetUserAdId());

            Assert.Equal("User does not have an objectidentifier claim.", exception.Message);
        }

        [Fact]
        public async Task GetUserTeam_ReturnsUsersProjectTeam_WhenUserHasTeam()
        {
            // Arrange
            var userAdId = "test-ad-id";

            var claims = new[] { new Claim("objectidentifier", userAdId) };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var mockSender = new Mock<ISender>();
            UserDto? userDto = new UserDto { Team = "business_support" };
            var userResult = Result<UserDto?>.Success(userDto);

            mockSender
                .Setup(s => s.Send(It.Is<GetUserByAdIdQuery>(q => q.UserAdId == userAdId), default))
                .ReturnsAsync(userResult);

            // Act
            var actualTeam = await claimsPrincipal.GetUserTeam(mockSender.Object);

            // Assert
            Assert.Equal(ProjectTeam.BusinessSupport, actualTeam);
        }

        [Fact]
        public void HasRole_ReturnsFalse_WhenNoClaimExists()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = claimsPrincipal.HasRole("my-role");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasRole_ReturnsTrue_WhenClaimExists()
        {
            // Arrange
            var claims = new[] { new Claim(ClaimTypes.Role, "important-role") };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

            // Act
            var result = claimsPrincipal.HasRole("important-role");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void HasRole_ReturnsFalse_WhenUserIsNull()
        {
            ClaimsPrincipal? user = null;
            Assert.False(user.HasRole("Admin"));
        }
    }
}
