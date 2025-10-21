using Microsoft.Extensions.Caching.Distributed;
using NSubstitute;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dfe.Complete.Tests.Extensions
{
    public class DistributedCacheExtensionsTests
    {
        private class MockObject
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }

        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null,
            WriteIndented = true,
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };       

        [Fact]
        public void TryGetValue_ReturnsDeserializedObject_IfExists()
        {
            // Arrange
            var cache = Substitute.For<IDistributedCache>();
            var key = "sample-key";
            var original = new MockObject { Name = "Bob", Age = 42 };
            var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(original, serializerOptions));

            cache.Get(key).Returns(jsonBytes);

            // Act
            var result = Dfe.Complete.Extensions.DistributedCacheExtensions.TryGetValue<MockObject>(cache, key, out var deserialized);

            // Assert
            Assert.True(result);
            Assert.NotNull(deserialized);
            Assert.Equal(original.Name, deserialized!.Name);
            Assert.Equal(original.Age, deserialized.Age);
        }

        [Fact]
        public void TryGetValue_ReturnsFalse_WhenKeyNotFound()
        {
            // Arrange
            var cache = Substitute.For<IDistributedCache>();
            var key = "nonexistent-key";

            cache.Get(key).Returns((byte[]?)null);

            // Act
            var result = Dfe.Complete.Extensions.DistributedCacheExtensions.TryGetValue<MockObject>(cache, key, out var deserialized);

            // Assert
            Assert.False(result);
            Assert.Null(deserialized);
        }

        [Fact]
        public async Task GetOrSetAsync_ReturnsCachedValue_IfExists()
        {
            // Arrange
            var cache = Substitute.For<IDistributedCache>();
            var key = "cached-key";
            var expected = new MockObject { Name = "Cached", Age = 99 };
            var jsonBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(expected, serializerOptions));

            _ = cache.Get(key).Returns(jsonBytes);

            // Act
            var result = await Dfe.Complete.Extensions.DistributedCacheExtensions.GetOrSetAsync(cache, key, () =>
            {   
                return Task.FromResult(expected);
            });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.Name, result!.Name);
            Assert.Equal(expected.Age, result.Age);
        }

        [Fact]
        public async Task GetOrSetAsync_SetsValue_IfNotExists()
        {
            // Arrange
            var cache = Substitute.For<IDistributedCache>();
            var key = "missing-key";
            var expected = new MockObject { Name = "New", Age = 50 };

            _ = cache.Get(key).Returns((byte[]?)null);

            // Act
            var result = await Dfe.Complete.Extensions.DistributedCacheExtensions.GetOrSetAsync(cache, key, () =>
            {
                return Task.FromResult(expected);
            });

            // Assert
            Assert.NotNull(result);
            await cache.Received(1).SetAsync(
                key,
                Arg.Any<byte[]>(),
                Arg.Any<DistributedCacheEntryOptions>()
            );
        }
    }

}
