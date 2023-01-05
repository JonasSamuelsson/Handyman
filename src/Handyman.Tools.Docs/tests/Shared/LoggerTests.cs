using Handyman.Tools.Docs.Shared;
using Shouldly;
using Xunit;

namespace Handyman.Tools.Docs.Tests.Shared;

public class LoggerTests
{
    [Theory]
    [InlineData(LogLevel.Debug, "debug: test")]
    [InlineData(LogLevel.Info, "info: test")]
    [InlineData(LogLevel.Error, "error: test")]
    public void ShouldSupportLogLevelFormatting(LogLevel logLevel, string expected)
    {
        var logger = new TestLogger
        {
            LogLevelFormatter = x => $"{x.ToString().ToLowerInvariant()}: "
        };

        logger.Write(logLevel, "test");

        logger.Output.ShouldBe(new[] { expected });
    }

    [Fact]
    public void OnlyCreatingScopeShouldNotProduceAnyOutput()
    {
        var logger = new TestLogger();

        var scope = logger.Scope("test");

        logger.Output.ShouldBeEmpty();

        scope.Dispose();

        logger.Output.ShouldBeEmpty();
    }

    [Fact]
    public void ShouldSupportScopedOutput()
    {
        var logger = new TestLogger();

        logger.Write(LogLevel.Info, "a");

        using (logger.Scope("1"))
        {
            logger.Write(LogLevel.Info, "b");

            using (logger.Scope("2"))
            {
                logger.Write(LogLevel.Info, "c");
            }

            using (logger.Scope("3"))
            {
                // intentionally empty
            }

            logger.Write(LogLevel.Info, "d");
        }

        logger.Write(LogLevel.Info, "e");

        logger.Output.ShouldBe(new[]
        {
            "a",
            "1",
            "  b",
            "  2",
            "    c",
            "  d",
            "e"
        });
    }
}