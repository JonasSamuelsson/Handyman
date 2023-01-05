using Handyman.Tools.Docs.Shared;
using Shouldly;
using System.Linq;
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
            LogLevelFormatter = x => $"{x.ToString().ToLowerInvariant()}: ",
            Verbosity = Verbosity.Diagnostics
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

    [Theory]
    [InlineData(Verbosity.Diagnostics, LogLevel.Debug, true)]
    [InlineData(Verbosity.Diagnostics, LogLevel.Error, true)]
    [InlineData(Verbosity.Diagnostics, LogLevel.Info, true)]
    [InlineData(Verbosity.Normal, LogLevel.Debug, false)]
    [InlineData(Verbosity.Normal, LogLevel.Error, true)]
    [InlineData(Verbosity.Normal, LogLevel.Info, true)]
    [InlineData(Verbosity.Quiet, LogLevel.Debug, false)]
    [InlineData(Verbosity.Quiet, LogLevel.Error, false)]
    [InlineData(Verbosity.Quiet, LogLevel.Info, false)]
    public void ShouldFilterByLogLevel(Verbosity verbosity, LogLevel logLevel, bool shouldLog)
    {
        var logger = new TestLogger
        {
            Verbosity = verbosity
        };

        logger.Write(logLevel, "");

        logger.Output.Any().ShouldBe(shouldLog);
    }
}