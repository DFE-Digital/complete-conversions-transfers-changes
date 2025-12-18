using Dfe.Complete.Validators;
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
        [InlineData("GRP_12345678")]
        public void IsValid_ShouldReturnSuccess_WhenValidFormat(string groupReferenceNumber)
        {
            // Act
            var attribute = new GroupReferenceNumberAttribute(true, "_UkprnField");
            var result = attribute.GetValidationResult(groupReferenceNumber, new ValidationContext(new()));

            // Assert
            Assert.Null(result);
        }
    }
}