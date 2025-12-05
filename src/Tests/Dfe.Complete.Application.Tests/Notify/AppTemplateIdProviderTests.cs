using Dfe.Complete.Infrastructure.Notify;
using Microsoft.Extensions.Options;
using Xunit;

namespace Dfe.Complete.Application.Tests.Notify
{
    public class AppTemplateIdProviderTests
    {
        private readonly NotifyOptions _options;
        private readonly AppTemplateIdProvider _sut;

        public AppTemplateIdProviderTests()
        {
            _options = new NotifyOptions
            {
                ApiKey = "test-api-key",
                Email = new NotifyOptions.EmailOptions
                {
                    Templates = new Dictionary<string, string>
                    {
                        { "NewAccountAdded", "d55de8f1-ce5a-4498-8229-baac7c0ee45f" },
                        { "NewConversionProjectCreated", "ea4f72e4-f5bb-4b1a-b5f9-a94cc1840353" },
                        { "NewTransferProjectCreated", "b0df8e28-ea23-46c5-9a83-82abc6b29193" },
                        { "AssignedNotification", "ec6823ec-0aae-439b-b2f9-c626809b7c61" }
                    }
                }
            };

            var optionsMock = Options.Create(_options);
            _sut = new AppTemplateIdProvider(optionsMock);
        }

        [Theory]
        [InlineData("NewAccountAdded", "d55de8f1-ce5a-4498-8229-baac7c0ee45f")]
        [InlineData("NewConversionProjectCreated", "ea4f72e4-f5bb-4b1a-b5f9-a94cc1840353")]
        [InlineData("NewTransferProjectCreated", "b0df8e28-ea23-46c5-9a83-82abc6b29193")]
        [InlineData("AssignedNotification", "ec6823ec-0aae-439b-b2f9-c626809b7c61")]
        public void GetTemplateId_WithValidKey_ReturnsCorrectTemplateId(string templateKey, string expectedId)
        {
            // Act
            var result = _sut.GetTemplateId(templateKey);

            // Assert
            Assert.Equal(expectedId, result);
        }

        [Fact]
        public void GetTemplateId_WithInvalidKey_ThrowsKeyNotFoundException()
        {
            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _sut.GetTemplateId("NonExistentTemplate"));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GetTemplateId_WithNullOrEmptyKey_ThrowsArgumentException(string templateKey)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _sut.GetTemplateId(templateKey));
        }

        [Theory]
        [InlineData("NewAccountAdded", true)]
        [InlineData("NewConversionProjectCreated", true)]
        [InlineData("NonExistentTemplate", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void TemplateExists_WithVariousKeys_ReturnsCorrectResult(string templateKey, bool expected)
        {
            // Act
            var result = _sut.TemplateExists(templateKey);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Constructor_WithNullOptions_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AppTemplateIdProvider(null!));
        }
    }
}

