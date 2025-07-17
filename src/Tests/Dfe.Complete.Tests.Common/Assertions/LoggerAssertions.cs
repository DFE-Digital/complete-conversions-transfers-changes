using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Dfe.Complete.Tests.Common.Assertions;

public static class LoggerAssertions
{

    public static void ShouldHaveLogged(this ILogger logger, string expectedMessagePart, LogLevel logLevel)
    {
        logger.Received().Log(
            logLevel,
            Arg.Any<EventId>(),
            Arg.Is<object>(v => v.ToString()!.Contains(expectedMessagePart)),
            Arg.Any<Exception?>(),
            Arg.Any<Func<object, Exception?, string>>()
        );
    }

    public static void ShouldHaveLogged<T>(this ILogger<T> logger, string expectedMessagePart, LogLevel logLevel) =>
        ShouldHaveLogged((ILogger)logger, expectedMessagePart, logLevel);
}
