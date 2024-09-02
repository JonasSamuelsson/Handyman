using Handyman.AspNetCore.ETags;
using Shouldly;
using System;
using Xunit;

namespace Handyman.AspNetCore.Tests.ETags;

public class ETagUtilityTests
{
    [Fact]
    public void BytesToETag()
    {
        ETagUtility.ToETag(new byte[] { 0, 1, 2, 3 }).ShouldBe("W/\"010203\"");
        ETagUtility.ToETag(new byte[] { 1, 2, 3 }).ShouldBe("W/\"010203\"");
        ETagUtility.ToETag(new byte[] { 0 }).ShouldBe("W/\"0\"");
        ETagUtility.ToETag(Array.Empty<byte>()).ShouldBe("W/\"0\"");
    }

    [Fact]
    public void StringToETag()
    {
        ETagUtility.ToETag("W/\"010203\"").ShouldBe("W/\"010203\"");
        ETagUtility.ToETag("010203").ShouldBe("W/\"010203\"");
        ETagUtility.ToETag("0").ShouldBe("W/\"0\"");
        ETagUtility.ToETag("*").ShouldBe("*");
        Should.Throw<Exception>(() => ETagUtility.ToETag(""));
        Should.Throw<Exception>(() => ETagUtility.ToETag("W/\"\""));
    }

    [Fact]
    public void BytesToETagValue()
    {
        ETagUtility.ToETagValue(new byte[] { 0, 1, 2, 3 }).ShouldBe("010203");
        ETagUtility.ToETagValue(new byte[] { 1, 2, 3 }).ShouldBe("010203");
        ETagUtility.ToETagValue(new byte[] { 0 }).ShouldBe("0");
        ETagUtility.ToETagValue(Array.Empty<byte>()).ShouldBe("0");
    }

    [Fact]
    public void StringToETagValue()
    {
        ETagUtility.ToETagValue("W/\"010203\"").ShouldBe("010203");
        ETagUtility.ToETagValue("010203").ShouldBe("010203");
        ETagUtility.ToETagValue("0").ShouldBe("0");
        Should.Throw<Exception>(() => ETagUtility.ToETagValue(""));
        Should.Throw<Exception>(() => ETagUtility.ToETagValue("W/\"\""));
    }
}