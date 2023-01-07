using Handyman.Tools.Docs.Shared;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.Tools.Docs.Tests.Shared;

public class LoggerTests
{
    [Fact]
    public void ShouldNotWriteEmptyScopes()
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

        logger.Write(LogLineType.Error, "a");

        using (logger.Scope("1"))
        {
            logger.Write(LogLineType.Error, "b");

            using (logger.Scope("2"))
            {
                logger.Write(LogLineType.Error, "c");
            }

            using (logger.Scope("3"))
            {
                // intentionally empty
            }

            logger.Write(LogLineType.Error, "d");
        }

        logger.Write(LogLineType.Error, "e");

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
    [InlineData(Verbosity.Diagnostics, LogLineType.Debug, true)]
    [InlineData(Verbosity.Diagnostics, LogLineType.Error, true)]
    [InlineData(Verbosity.Diagnostics, LogLineType.Error, true)]
    [InlineData(Verbosity.Normal, LogLineType.Debug, false)]
    [InlineData(Verbosity.Normal, LogLineType.Error, true)]
    [InlineData(Verbosity.Normal, LogLineType.Error, true)]
    [InlineData(Verbosity.Quiet, LogLineType.Debug, false)]
    [InlineData(Verbosity.Quiet, LogLineType.Error, false)]
    [InlineData(Verbosity.Quiet, LogLineType.Error, false)]
    public void ShouldFilterByLogLevel(Verbosity verbosity, LogLineType logLineType, bool shouldLog)
    {
        var logger = new TestLogger
        {
            Verbosity = verbosity
        };

        logger.Write(logLineType, "");

        logger.Output.Any().ShouldBe(shouldLog);
    }
}