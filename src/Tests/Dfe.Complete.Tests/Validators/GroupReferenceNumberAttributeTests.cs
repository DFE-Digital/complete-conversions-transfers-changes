using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using Dfe.Complete.Extensions;
using Dfe.Complete.Validators;
using MediatR;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Dfe.Complete.Tests.Validators
{
    public class GroupReferenceNumberAttributeTests
    {
        [Theory]
        [InlineData("", "", "", "")]
        [InlineData(null, null, null, "")]
        [InlineData("GDP_", "", "", "A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001")]
        [InlineData("GRP_", "", "", "A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001")]
        [InlineData("GRP_ABC", "", "", "A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001")]
        [InlineData("GRP_1234567", "", "", "A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001")]
        [InlineData("GRP_123456789", "", "", "A group reference number must start GRP_ and contain 8 numbers, like GRP_00000001")]
        [InlineData("GRP_12345678", "", "", "Incoming trust ukprn cannot be empty")]
        [InlineData("GRP_12345678", "87654321", "11111111", "The group reference number must be for the same trust as all other group members, check the group reference number and incoming trust UKPRN")]
        [InlineData("GRP_12345678", "12345678", "12345678", "")]
        public async Task IsValid_ShouldReturnExpectedResults(
            string groupReferenceNumber,
            string ukprnPropertyValue,
            string expectedSenderResultUkprn,
            string expectedErrorMessage)
        {
            // Arrange
            var mockSender = new Mock<ISender>();
            
            if (ukprnPropertyValue != null)
            {
                if (ukprnPropertyValue == expectedSenderResultUkprn)
                {
                    mockSender.Setup(s => s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(Result<ProjectGroupDto>.Success(new ProjectGroupDto() { TrustUkprn = new Ukprn(expectedSenderResultUkprn.ToInt()) }));
                }
                else
                {
                    mockSender.Setup(s => s.Send(It.IsAny<GetProjectGroupByGroupReferenceNumberQuery>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync(Result<ProjectGroupDto>.Success(null));
                }
            }

            var objectInstance = new
            {
                _UkprnField = ukprnPropertyValue
            };

            var attribute = new GroupReferenceNumberAttribute(true, nameof(objectInstance._UkprnField));
            var validationContext = new ValidationContext(objectInstance, null, null)
            {
                //MemberName = nameof(objectInstance._UkprnField),
                
            };
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