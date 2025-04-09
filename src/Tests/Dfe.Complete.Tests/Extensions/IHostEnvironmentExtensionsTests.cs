using Dfe.Complete.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Dfe.Complete.Tests.Extensions;

public class IHostEnvironmentExtensionsTests
{
    [Fact]
    public void IsTest_ReturnsTrue_WhenEnvIsTest()
    {
        var mock = new Mock<IHostEnvironment>();
        mock.Setup(env => env.EnvironmentName).Returns("Test");

        var result = mock.Object.IsTest();

        Assert.True(result);
    }

    [Fact]
    public void IsTest_ReturnsFalse_WhenEnvIsNotTest()
    {
        var mock = new Mock<IHostEnvironment>();
        mock.Setup(env => env.EnvironmentName).Returns("Production");

        var result = mock.Object.IsTest();

        Assert.False(result);
    }

    [Fact]
    public void IsTest_ThrowsException_WhenHostIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
        {
            IHostEnvironment env = null!;
            env.IsTest();
        });
    }
}