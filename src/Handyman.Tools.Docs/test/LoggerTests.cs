using Shouldly;
using System.Linq;
using Handyman.Tools.Docs.Utils;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class LoggerTests
    {
        [Fact]
        public void ShouldWriteError()
        {
            var writer = new TestConsoleWriter();
            var logger = new Logger(writer);

            logger.WriteError("foo");

            writer.Messages.Single().ShouldBe("e:foo");
        }

        [Fact]
        public void ShouldWriteInfo()
        {
            var writer = new TestConsoleWriter();
            var logger = new Logger(writer);

            logger.WriteInfo("foo");

            writer.Messages.Single().ShouldBe("i:foo");
        }

        [Fact]
        public void ShouldHandlePrefixes()
        {
            var writer = new TestConsoleWriter();
            var logger = new Logger(writer);

            logger.WriteError("one");
            logger.WriteInfo("one");

            using (logger.UsePrefix("1"))
                ;

            logger.WriteError("two");
            logger.WriteInfo("two");

            using (logger.UsePrefix("2"))
            {
                logger.WriteError("three");
                logger.WriteInfo("three");

                using (logger.UsePrefix("3"))
                {
                    logger.WriteError("four");
                    logger.WriteInfo("four");
                }

                logger.WriteError("five");
                logger.WriteInfo("five");
            }

            logger.WriteError("six");
            logger.WriteInfo("six");

            writer.Messages.ShouldBe(new[]
            {
                "e:one",
                "i:one",
                "e:two",
                "i:two",
                "e:2three",
                "i:2three",
                "e:23four",
                "i:23four",
                "e:2five",
                "i:2five",
                "e:six",
                "i:six"
            });
        }

        [Fact]
        public void ShouldHandleScopes()
        {
            var writer = new TestConsoleWriter();
            var logger = new Logger(writer);

            logger.WriteError("one");
            logger.WriteInfo("one");

            using (logger.CreateScope("1"))
                ;

            logger.WriteError("two");
            logger.WriteInfo("two");

            using (logger.CreateScope("2"))
            {
                logger.WriteError("three");
                logger.WriteInfo("three");

                using (logger.CreateScope("3"))
                {
                    logger.WriteError("four");
                    logger.WriteInfo("four");
                }

                logger.WriteError("five");
                logger.WriteInfo("five");
            }

            logger.WriteError("six");
            logger.WriteInfo("six");

            writer.Messages.ShouldBe(new[]
            {
                "e:one",
                "i:one",
                "e:two",
                "i:two",
                "i:2",
                "e:  three",
                "i:  three",
                "i:  3",
                "e:    four",
                "i:    four",
                "e:  five",
                "i:  five",
                "e:six",
                "i:six"
            });
        }
    }
}