using Handyman.AspNetCore.ETags;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Shouldly;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.AspNetCore.Tests.ETags
{
    [ApiController, Route("eTags")]
    public class TestServerTests : ControllerBase
    {
        [Fact]
        public async Task ShouldReadETagsFromHeaders()
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
                Headers =
                {
                    {HeaderNames.IfMatch, "W/\"a\"" },
                    {HeaderNames.IfNoneMatch, "W/\"b\"" },
                },
                Method = HttpMethod.Get,
                RequestUri = new Uri("eTags/optional", UriKind.Relative)
            };

            var response = await client.SendAsync(request);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var json = await response.Content.ReadAsStringAsync();
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            dictionary["ifMatch"].ShouldBe("W/\"a\"");
            dictionary["ifMatchETag"].ShouldBe("W/\"a\"");
            dictionary["ifNoneMatch"].ShouldBe("W/\"b\"");
            dictionary["ifNoneMatchETag"].ShouldBe("W/\"b\"");
        }

        [Theory]
        [InlineData("If-Match")]
        [InlineData("If-None-Match")]
        public async Task ShouldReturn400ResponseIfEtagHasInvalidFormat(string headerName)
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
                RequestUri = new Uri("eTags/optional", UriKind.Relative)
            };

            request.Headers.TryAddWithoutValidation(headerName, "invalid");

            var response = await client.SendAsync(request);

            response.Content.Headers.ContentType.CharSet.ShouldBe("utf-8");
            response.Content.Headers.ContentType.MediaType.ShouldBe("application/problem+json");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync();
            var details = JsonConvert.DeserializeObject<ProblemDetails>(json);

            // since we are using the built in model binding/validation we only assert the status code

            //details.Detail.ShouldBe("Invalid ETag format");
            details.Status.ShouldBe(StatusCodes.Status400BadRequest);
            //details.Title.ShouldBe("Bad Request");
            //details.Type.ShouldBe("https://httpstatuses.com/400");
        }

        [Fact]
        public async Task ShouldReturn400ResponseIfRequiredETagIsNotProvided()
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
                RequestUri = new Uri("eTags/required", UriKind.Relative)
            };

            request.Headers.TryAddWithoutValidation(HeaderNames.IfMatch, "fail");

            var response = await client.SendAsync(request);

            response.Content.Headers.ContentType.CharSet.ShouldBe("utf-8");
            response.Content.Headers.ContentType.MediaType.ShouldBe("application/problem+json");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync();
            var details = JsonConvert.DeserializeObject<ProblemDetails>(json);

            // since we are using the built in model binding/validation we only assert the status code

            //details.Detail.ShouldBe("Invalid ETag format");
            details.Status.ShouldBe(StatusCodes.Status400BadRequest);
            //details.Title.ShouldBe("Bad Request");
            //details.Type.ShouldBe("https://httpstatuses.com/400");
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
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }
        }

        [HttpGet("optional")]
        public object Get(string ifMatch, string ifMatchETag, string ifNoneMatch, string ifNoneMatchETag)
        {
            return new
            {
                ifMatch,
                ifMatchETag,
                ifNoneMatch,
                ifNoneMatchETag
            };
        }

        [HttpGet("required")]
        public void GetRequired([Required]string ifMatch) { }
    }
}