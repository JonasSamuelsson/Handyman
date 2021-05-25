using Handyman.AspNetCore.ETags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shouldly;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.AspNetCore.Tests.ETags
{
    [ApiController, Route("eTags")]
    public class TestServerTests : ControllerBase
    {
        [Theory]
        [InlineData("ifMatch", null, null, null, HttpStatusCode.PreconditionRequired)]
        [InlineData("ifMatch", "if-match", "fail", "W/\"pass\"", HttpStatusCode.BadRequest)]
        [InlineData("ifMatch", "if-match", "W/\"fail\"", "W/\"pass\"", HttpStatusCode.PreconditionFailed)]
        [InlineData("ifMatch", "if-match", "W/\"pass\"", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("ifMatch", "if-match", "*", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("ifMatchETag", null, null, null, HttpStatusCode.PreconditionRequired)]
        [InlineData("ifMatchETag", "if-match", "fail", "W/\"pass\"", HttpStatusCode.BadRequest)]
        [InlineData("ifMatchETag", "if-match", "W/\"fail\"", "W/\"pass\"", HttpStatusCode.PreconditionFailed)]
        [InlineData("ifMatchETag", "if-match", "W/\"pass\"", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("ifMatchETag", "if-match", "*", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("IfNoneMatch", null, null, null, HttpStatusCode.OK)]
        [InlineData("ifNoneMatch", "if-none-match", "fail", "W/\"pass\"", HttpStatusCode.BadRequest)]
        [InlineData("ifNoneMatch", "if-none-match", "W/\"fail\"", "W/\"pass\"", HttpStatusCode.PreconditionFailed)]
        [InlineData("ifNoneMatch", "if-none-match", "W/\"pass\"", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("ifNoneMatch", "if-none-match", "*", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("IfNoneMatchETag", null, null, null, HttpStatusCode.OK)]
        [InlineData("ifNoneMatchETag", "if-none-match", "fail", "W/\"pass\"", HttpStatusCode.BadRequest)]
        [InlineData("ifNoneMatchETag", "if-none-match", "W/\"fail\"", "W/\"pass\"", HttpStatusCode.PreconditionFailed)]
        [InlineData("ifNoneMatchETag", "if-none-match", "W/\"pass\"", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("ifNoneMatchETag", "if-none-match", "*", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("custom", null, null, null, HttpStatusCode.PreconditionRequired)]
        [InlineData("custom", "if-match", "fail", "W/\"pass\"", HttpStatusCode.BadRequest)]
        [InlineData("custom", "if-match", "W/\"fail\"", "W/\"pass\"", HttpStatusCode.PreconditionFailed)]
        [InlineData("custom", "if-match", "W/\"pass\"", "W/\"pass\"", HttpStatusCode.OK)]
        [InlineData("custom", "if-match", "*", "W/\"pass\"", HttpStatusCode.OK)]
        public async Task ShouldTest(string path, string headerName, string headerValue, string expectedETag, HttpStatusCode expectedStatusCode)
        {
            var builder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                });

            var host = await builder.StartAsync();

            var client = host.GetTestClient();

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"eTags/{path}?expectedETag={expectedETag}", UriKind.Relative)
            };

            if (headerValue != null)
            {
                request.Headers.TryAddWithoutValidation(headerName, headerValue);
            }

            var response = await client.SendAsync(request);

            response.StatusCode.ShouldBe(expectedStatusCode);
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddETags();
                services.AddControllers();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseRouting();
                app.UseETags();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }
        }

        [HttpGet("ifMatch")]
        public void IfMatch(string ifMatch, string expectedETag, [FromServices] IETagComparer comparer)
        {
            Compare(ifMatch, expectedETag, comparer);
        }

        [HttpGet("ifMatchETag")]
        public void IfMatchETag(string ifMatchETag, string expectedETag, [FromServices] IETagComparer comparer)
        {
            Compare(ifMatchETag, expectedETag, comparer);
        }

        [HttpGet("ifNoneMatch")]
        public void IfNoneMatch(string ifNoneMatch, string expectedETag, [FromServices] IETagComparer comparer)
        {
            Compare(ifNoneMatch, expectedETag, comparer);
        }

        [HttpGet("ifNoneMatchETag")]
        public void IfNoneMatchETag(string ifNoneMatchETag, string expectedETag, [FromServices] IETagComparer comparer)
        {
            Compare(ifNoneMatchETag, expectedETag, comparer);
        }

        [HttpGet("custom")]
        public void Custom([FromIfMatchHeader] string input, string expectedETag, [FromServices] IETagComparer comparer)
        {
            Compare(input, expectedETag, comparer);
        }

        private static void Compare(string headerETag, string expectedETag, IETagComparer comparer)
        {
            if (expectedETag == null)
                return;

            comparer.EnsureEquals(headerETag, expectedETag);
        }
    }
}