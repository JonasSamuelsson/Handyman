using Shouldly;
using Xunit;

namespace Handyman.Tests.UriBuilder
{
    public class UriBuilder_tests
    {
        [Fact]
        public void should_construct_url()
        {
            new Handyman.UriBuilder()
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
            new Handyman.UriBuilder()
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
            new Handyman.UriBuilder()
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

        [Fact]
        public void should_use_http_as_default_scheme()
        {
            new Handyman.UriBuilder()
                  .Host("test.com")
                  .ToString()
                  .ShouldBe("http://test.com");
        }

        [Fact]
        public void should_use_DefaultScheme()
        {
            try
            {
                Handyman.UriBuilder.DefaultScheme = "ftp";
                new Handyman.UriBuilder()
                      .Host("test.com")
                      .ToString()
                      .ShouldBe("ftp://test.com");
            }
            finally
            {
                Handyman.UriBuilder.DefaultScheme = null;
            }
        }
    }
}
