using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Validators;
using MediatR;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators
{
    public class GroupReferenceNumberAttributeTests
    {
        [Theory]
        [InlineData("GDP_")]
        [InlineData("GRP_")]
        [InlineData("GRP_ABC")]
        [InlineData("GRP_1234567")]
        [InlineData("GRP_123456789")]
        public void Isvalid_ShouldReturnErrorMessage_WhenInvalidFormat(string groupReferenceNumber)
        {
            var attribute = new GroupReferenceNumberAttribute();
            var result = attribute.GetValidationResult(groupReferenceNumber, new(new()));
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Equal("A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001", result.ErrorMessage);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("GRP_12345678")]
        public void Isvalid_ShouldReturnErrorMessage_WhenValidFormat(string groupReferenceNumber)
        {
            var attribute = new GroupReferenceNumberAttribute();
            var result = attribute.GetValidationResult(groupReferenceNumber, new(new()));
            Assert.Null(result);
        }

        [Theory]
        [InlineData("GRP_12345678", "", "Incoming trust ukprn cannot be empty")]
        [InlineData("GRP_12345678", "87654321", "The group reference number must be for the same trust as all other group members, check the group reference number and incoming trust UKPRN")]
        [InlineData("GRP_12345678", "12345678", "")]
        public void IsValid_ShouldReturnExpectedResults(
            string groupReferenceNumber,
            string ukprnPropertyValue,
            string expectedErrorMessage)
        {
            // Arrange
            var mockSender = new Mock<ISender>();

            mockSender.Setup(s => s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<ProjectGroupDto>.Success(new ProjectGroupDto() { TrustUkprn = new Ukprn(12345678) })!);

            var objectInstance = new { _UkprnField = ukprnPropertyValue };
            var attribute = new GroupReferenceNumberAttribute(true, nameof(objectInstance._UkprnField));
            var validationContext = new ValidationContext(objectInstance, null, null) { };

            validationContext.InitializeServiceProvider(type =>
                type == typeof(ISender) ? mockSender.Object : null);

            // Act
            var result = attribute.GetValidationResult(groupReferenceNumber, validationContext);

            if (string.IsNullOrEmpty(expectedErrorMessage))
            {
                Assert.Null(result);
            }
            else
            {
                Assert.NotNull(result);
                Assert.IsType<ValidationResult>(result);
                Assert.Equal(expectedErrorMessage, result.ErrorMessage);
            }
        }
    }
}