using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DistributedCacheExtensions = Dfe.Complete.Application.Extensions.DistributedCacheExtensions;

namespace Dfe.Complete.Application.Tests.Extensions;

    public class DistributedCacheExtensionsTests
    {
        private readonly IFixture _fixture;
        private readonly IDistributedCache _cache;
        private readonly JsonSerializerOptions _options;

        public DistributedCacheExtensionsTests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            _cache = _fixture.Freeze<IDistributedCache>();
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = null,
                WriteIndented = true,
                AllowTrailingCommas = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
        }        
        
        [Fact]
        public void TryGetValue_ReturnsDeserializedObject_IfExists()
        {
            // Arrange
            var key = _fixture.Create<string>();
            var value = _fixture.Create<MockData>();
            var json = JsonSerializer.Serialize(value, _options);
            var bytes = Encoding.UTF8.GetBytes(json);

            _cache.Get(key).Returns(bytes);

            // Act
            var result = DistributedCacheExtensions.TryGetValue<MockData>(_cache, key, out var deserialized);

            // Assert
            Assert.True(result);
            Assert.NotNull(deserialized);
            Assert.Equal(value.Name, deserialized.Name);
            Assert.Equal(value.Age, deserialized.Age);
        }

        [Fact]
        public void TryGetValue_ReturnsFalse_WhenKeyNotFound()
        {
            // Arrange
            var key = _fixture.Create<string>();

            _cache.Get(key).Returns((byte[]?)null);

            // Act
            var result = DistributedCacheExtensions.TryGetValue<MockData>(_cache, key, out var deserialized);

            // Assert
            Assert.False(result);
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task GetOrSetAsync_ReturnsCachedValue_IfExists()
        {
            // Arrange            
            var key = _fixture.Create<string>();
            var value = _fixture.Create<MockData>();
            var json = JsonSerializer.Serialize(value, _options);
            var bytes = Encoding.UTF8.GetBytes(json);

            _cache.Get(key).Returns(bytes);            

            var result = await DistributedCacheExtensions.GetOrSetAsync(_cache, key, () =>
            {
                return Task.FromResult(value);
            });


            // Assert
            Assert.NotNull(result);
            Assert.Equal(value.Name, result!.Name);
            Assert.Equal(value.Age, result.Age);

        }

        [Fact]
        public async Task GetOrSetAsync_SetsValue_IfNotExists()
        {
            // Arrange                        
            var key = _fixture.Create<string>();
            var value = _fixture.Create<MockData>();
            var json = JsonSerializer.Serialize(value, _options);
            var bytes = Encoding.UTF8.GetBytes(json);
            
            _cache.Get(key).Returns((byte[]?)null);

            // Act
             var result = await DistributedCacheExtensions.GetOrSetAsync(_cache, key, () =>
            {
                return Task.FromResult(value);
            });

            // Assert
            Assert.NotNull(result);
            await _cache.Received(1).SetAsync(
                key,
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>()
            );
        }

        private class MockData
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }
}
  

