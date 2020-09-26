using Handyman.Tools.Docs.Utils;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class ElementParserTests
    {
        [Fact]
        public void ShouldParse()
        {
            var lines = new[]
            {
                "// some comment",
                "// <handyman-docs:import-code src=\"foo/bar.txt\" lines=\"3-6\">",
                "public class Foobar { }",
                "// </handyman-docs:import-code>"
            };

            var elements = new ElementsParser(Enumerable.Empty<IAttributesParser>()).Parse("import-code", lines);

            var element = elements.Single();

            element.Name.ShouldBe("import-code");

            element.Attributes.Length.ShouldBe(2);
            element.Attributes.Single(x => x.Name == "src").Value.ShouldBe("foo/bar.txt");
            element.Attributes.Single(x => x.Name == "lines").Value.ShouldBe("3-6");
        }
    }
}
