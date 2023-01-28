using Handyman.Tools.Docs.Shared;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.Tools.Docs.Tests.Shared;

public class LinesSpecParserTests
{
    [Fact]
    public void ShouldParseSingleLineSectionFromValue()
    {
        new LinesSpecParser()
            .TryParse("3", typeof(LinesSpec), out object value)
            .ShouldBeTrue();

        var linesSpec = value.ShouldBeAssignableTo<LinesSpec>();

        linesSpec.Sections.Single().Count.ShouldBe(1);
        linesSpec.Sections.Single().FromNumber.ShouldBe(3);
    }

    [Fact]
    public void ShouldParseSingleLineSectionFromRange()
    {
        new LinesSpecParser()
            .TryParse("3-3", typeof(LinesSpec), out object value)
            .ShouldBeTrue();

        var linesSpec = value.ShouldBeAssignableTo<LinesSpec>();

        linesSpec.Sections.Single().Count.ShouldBe(1);
        linesSpec.Sections.Single().FromNumber.ShouldBe(3);
    }

    [Fact]
    public void ShouldParseSectionFromRange()
    {
        new LinesSpecParser()
            .TryParse("3-5", typeof(LinesSpec), out object value)
            .ShouldBeTrue();

        var linesSpec = value.ShouldBeAssignableTo<LinesSpec>();

        linesSpec.Sections.Single().Count.ShouldBe(3);
        linesSpec.Sections.Single().FromNumber.ShouldBe(3);
    }

    [Fact]
    public void ShouldParseSections()
    {
        new LinesSpecParser()
            .TryParse("3-5,7", typeof(LinesSpec), out object value)
            .ShouldBeTrue();

        var linesSpec = value.ShouldBeAssignableTo<LinesSpec>();

        linesSpec.Sections.Count.ShouldBe(2);

        linesSpec.Sections[0].Count.ShouldBe(3);
        linesSpec.Sections[0].FromNumber.ShouldBe(3);

        linesSpec.Sections[1].Count.ShouldBe(1);
        linesSpec.Sections[1].FromNumber.ShouldBe(7);
    }
}