using System.ComponentModel.DataAnnotations;
using Dfe.Complete.Validators;
using Dfe.Complete.Application.Projects.Queries.GetProject;
using Dfe.Complete.Domain.ValueObjects;
using MediatR;
using Moq;
using Dfe.Complete.Domain.Entities;
using Dfe.Complete.Application.Common.Models;
using Dfe.Complete.Application.Projects.Models;
using AutoMapper;
using Dfe.AcademiesApi.Client.Contracts;

namespace Dfe.Complete.Tests.Validators
{
    public class UrnAttributeTests
    {
        [Theory]
        [InlineData(null, true)]       // Null value
        [InlineData("", true)]         // Empty string
        [InlineData("12345", false)]   // Less than 6 digits
        [InlineData("1234567", false)] // More than 6 digits
        [InlineData("123456", false)]   // Valid URN (not existing)
        [InlineData("133456", true)]   // Valid URN (not existing)
        public void UrnAttribute_Validation_ReturnsExpectedResult(string urn, bool expectedIsValid)
        {
            // Arrange
            var mockSender = new Mock<ISender>();
            var mockMapper = new Mock<IMapper>();
            var mockEstablishmentClient = new Mock<IEstablishmentsV4Client>();

            mockEstablishmentClient
                .Setup(service => service.GetEstablishmentByUrnAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<EstablishmentDto>());

            var projectDtoToReturn = urn == "123456" ? new ProjectDto() : null;

            mockSender
                .Setup(sender => sender.Send(It.IsAny<GetProjectByUrnQuery>(), default))
                .ReturnsAsync(Result<ProjectDto>.Success(projectDtoToReturn));

            mockMapper
                .Setup(m => m.Map<ProjectDto>(It.IsAny<Project>()))
                .Returns(projectDtoToReturn);

            var attribute = new UrnAttribute();
            var validationContext = new ValidationContext(new { }, null, null)
            {
                MemberName = "TestUrn"
            };
            validationContext.InitializeServiceProvider(type =>
            {
                if (type == typeof(ISender))
                    return mockSender.Object;
                if (type == typeof(IEstablishmentsV4Client))
                    return mockEstablishmentClient.Object;
                return null;
            });
            
            // Act
            var result = attribute.GetValidationResult(urn, validationContext);

            // Assert
            if (expectedIsValid)
            {
                Assert.Null(result); // Success returns null
            }
            else
            {
                Assert.NotNull(result); // Failure returns ValidationResult
                Assert.IsType<ValidationResult>(result);
            }
        }

        [Fact]
        public void UrnAttribute_Validation_Fails_WhenUrnAlreadyExists()
        {
            // Arrange
            var mockSender = new Mock<ISender>();
            var mockEstablishmentClient = new Mock<IEstablishmentsV4Client>();

            var urnValue = 123456;

            mockSender
                .Setup(sender => sender.Send(It.IsAny<GetProjectByUrnQuery>(), default))
                    .ReturnsAsync(Result<ProjectDto?>.Success(new ProjectDto() { Urn = new Urn(urnValue)}));

            mockEstablishmentClient
                .Setup(service => service.GetEstablishmentByUrnAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<EstablishmentDto>());

            var attribute = new UrnAttribute();
            var validationContext = new ValidationContext(new { }, null, null)
            {
                MemberName = "TestUrn"
            };
            validationContext.InitializeServiceProvider(type =>
            {
                if (type == typeof(ISender))
                    return mockSender.Object;
                if (type == typeof(IEstablishmentsV4Client))
                    return mockEstablishmentClient.Object;
                return null;
            });
            // Act
            var result = attribute.GetValidationResult(urnValue.ToString(), validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Equal($"A project with the urn: {urnValue} already exists", result.ErrorMessage);
        }
        
        [Fact]
        public void UrnAttribute_Validation_Throws_Exception_WhenResultIsFalse()
        {
            // Arrange
            var mockSender = new Mock<ISender>();
            var mockEstablishmentClient = new Mock<IEstablishmentsV4Client>();

            var urnValue = 123456;

            var expectedErrorMessage = "Error Message";
            
            mockSender
                .Setup(sender => sender.Send(It.IsAny<GetProjectByUrnQuery>(), default))
                    .ReturnsAsync(Result<ProjectDto?>.Failure(expectedErrorMessage));
            
            mockEstablishmentClient
                .Setup(service => service.GetEstablishmentByUrnAsync(It.IsAny<string>()))
                .ReturnsAsync(It.IsAny<EstablishmentDto>());


            var attribute = new UrnAttribute();
            var validationContext = new ValidationContext(new { }, null, null)
            {
                MemberName = "TestUrn"
            };
            validationContext.InitializeServiceProvider(type =>
            {
                if (type == typeof(ISender))
                    return mockSender.Object;
                if (type == typeof(IEstablishmentsV4Client))
                    return mockEstablishmentClient.Object;
                return null;
            });
            // Act
            var exception = Assert.Throws<Exception>(() => attribute.GetValidationResult(urnValue.ToString(), validationContext));

            // Assert
            //Assert(result);
            Assert.Equal(expectedErrorMessage, exception.Message);
        }

        [Fact]
        public void UrnAttribute_Validation_Fails_WhenUrnDoesNotExistInAcademies()
        {
            // Arrange
            var mockEstablishmentClient = new Mock<IEstablishmentsV4Client>();

            var urnValue = "123456";

            mockEstablishmentClient
                .Setup(service => service.GetEstablishmentByUrnAsync(urnValue))
                .Throws(new AggregateException());

            var attribute = new UrnAttribute();
            var validationContext = new ValidationContext(new { }, null, null)
            {
                MemberName = "TestUrn"
            };
            validationContext.InitializeServiceProvider(type => type == typeof(IEstablishmentsV4Client) ? mockEstablishmentClient.Object : null);

            // Act
            var result = attribute.GetValidationResult(urnValue, validationContext);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ValidationResult>(result);
            Assert.Equal($"There's no school or academy with that URN. Check the number you entered is correct.", result.ErrorMessage);
        }
    }
}
