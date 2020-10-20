using Handyman.Tools.Docs.ImportCode;
using Handyman.Tools.Docs.Utils;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Handyman.Tools.Docs.Tests.Utils
{
    public class CodeBlockDataSerializationTests
    {
        [Fact]
        public void ShouldDeserializeFromSource()
        {
            var keyValuePairs = new Dictionary<string, string> { { "source", "foo.cs" } }
                .ToList();

            new DataSerializer<CodeBlockData>()
                .TryDeserialize(keyValuePairs, new TestLogger(), out var data)
                .ShouldBeTrue();

            data.Source.ShouldBe("foo.cs");
        }

        [Fact]
        public void ShouldDeserializeFromSourceAndId()
        {
            var keyValuePairs = new Dictionary<string, string>
                {
                    { "source", "foo.cs" },
                    { "id", "bar" },
                }
                .ToList();

            new DataSerializer<CodeBlockData>()
                .TryDeserialize(keyValuePairs, new TestLogger(), out var data)
                .ShouldBeTrue();

            data.Source.ShouldBe("foo.cs");
            data.Id.ShouldBe("bar");
        }

        [Fact]
        public void ShouldDeserializeFromSourceAndLines()
        {
            var keyValuePairs = new Dictionary<string, string>
                {
                    { "source", "foo.cs" },
                    { "lines", "8" },
                }
                .ToList();

            new DataSerializer<CodeBlockData>()
                .TryDeserialize(keyValuePairs, new TestLogger(), out var data)
                .ShouldBeTrue();

            data.Source.ShouldBe("foo.cs");
            data.Lines.FromNumber.ShouldBe(8);
        }

        [Fact]
        public void ShouldDeserializeFromSourceAndLanguage()
        {
            var keyValuePairs = new Dictionary<string, string>
                {
                    { "source", "foo.cs" },
                    { "language", "txt" },
                }
                .ToList();

            new DataSerializer<CodeBlockData>()
                .TryDeserialize(keyValuePairs, new TestLogger(), out var data)
                .ShouldBeTrue();

            data.Source.ShouldBe("foo.cs");
            data.Language.ShouldBe("txt");
        }

        [Fact]
        public void ShouldNotDeserializeIfSourceIsMissing()
        {
            var keyValuePairs = new Dictionary<string, string>()
                .ToList();

            new DataSerializer<CodeBlockData>()
                .TryDeserialize(keyValuePairs, new TestLogger(), out var data)
                .ShouldBeFalse();
        }

        [Fact]
        public void ShouldNotDeserializeIfBothIdAndLinesAreProvided()
        {
            var keyValuePairs = new Dictionary<string, string>
                {
                    { "source", "foo.cs" },
                    { "id", "x" },
                    { "lines", "1" }
                }
                .ToList();

            new DataSerializer<CodeBlockData>()
                .TryDeserialize(keyValuePairs, new TestLogger(), out var data)
                .ShouldBeFalse();
        }
    }
}