using System.Linq;
using Shouldly;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class LoggerTests
    {
        [Fact]
        public void ShouldWriteError()
        {
            var logger = new TestLogger();

            logger.WriteError("foo");

            logger.Messages.Single().ShouldBe("e:foo");
        }

        [Fact]
        public void ShouldWriteInfo()
        {
            var logger = new TestLogger();

            logger.WriteInfo("foo");

            logger.Messages.Single().ShouldBe("i:foo");
        }

        [Fact]
        public void ShouldHandlePrefixes()
        {
            var logger = new TestLogger();

            logger.WriteInfo("one");

            using (logger.UsePrefix("-"))
            {
                logger.WriteError("two");
                logger.WriteInfo("two");
            }

            logger.WriteInfo("three");

            logger.Messages.ShouldBe(new[]
            {
                "i:one",
                "e:-two",
                "i:-two",
                "i:three"
            });
        }

        [Fact]
        public void ShouldHandleScopes()
        {
            var logger = new TestLogger();

            logger.WriteInfo("one");

            using (logger.CreateScope("1"))
            {
                logger.WriteError("two");

                using (logger.CreateScope("2"))
                    logger.WriteInfo("three");
            }

            logger.WriteInfo("four");

            logger.Messages.ShouldBe(new[]
            {
                "i:one",
                "i: > 1",
                "e:   two",
                "i:    > 2",
                "i:      three",
                "i:four"
            });
        }
    }
}