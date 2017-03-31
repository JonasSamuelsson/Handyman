using Handyman.Net;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Net
{
    public class UriBuilder_tests
    {
        [Fact]
        public void should_construct_url()
        {
            new UriBuilder()
               .Scheme("ftp")
               .Host("foo.com")
               .Port(123)
               .Path("alpha/beta")
               .Path("gamma")
               .Query("number=1")
               .Fragment("anchor")
               .ToString()
               .ShouldBe("ftp://foo.com:123/alpha/beta/gamma?number=1#anchor");
        }

        [Fact]
        public void should_construct_url_from_baseaddress()
        {
            new UriBuilder()
               .BaseAddress("ftp://foo.com:123")
               .Path("alpha/beta")
               .Path("gamma")
               .Query("number=1")
               .Fragment("anchor")
               .ToString()
               .ShouldBe("ftp://foo.com:123/alpha/beta/gamma?number=1#anchor");
        }

        [Fact]
        public void should_encode_querystring()
        {
            new UriBuilder()
               .Scheme("ftp")
               .Host("foo.com")
               .Port(123)
               .Path("alpha/beta")
               .Query("number=1")
               .QueryParams("text", "hello coderz")
               .Fragment("anchor")
               .ToString()
               .ShouldBe("ftp://foo.com:123/alpha/beta?number=1&text=hello+coderz#anchor");
        }
    }
}
