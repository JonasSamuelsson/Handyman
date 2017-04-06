using Shouldly;
using Xunit;

namespace Handyman.Tests.UriBuilder
{
    public class UriBuilder_base_address_tests
    {
        [Fact]
        public void should_construct_uri_from_base_address_alone()
        {
            new Handyman.UriBuilder()
               .BaseAddress("ftp://base.io/foo")
               .ToString()
               .ShouldBe("ftp://base.io/foo");
        }

        [Fact]
        public void should_construct_uri_from_base_address_with_overrides()
        {
            new Handyman.UriBuilder()
               .BaseAddress("ftp://base.io/foo")
               .Scheme("https")
               .Host("test.com")
               .Path("bar")
               .Query("a=1")
               .QueryParams("b", 2)
               .QueryParams("c")
               .ToString()
               .ShouldBe("https://test.com/foo/bar?a=1&b=2&c");
        }
    }
}
