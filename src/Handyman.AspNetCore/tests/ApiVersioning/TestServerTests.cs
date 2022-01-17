using Handyman.AspNetCore.ApiVersioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Shouldly;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning
{
    [ApiController, Route("api-versioning")]
    public class TestServerTests : ControllerBase
    {
        [Theory]
        [InlineData("single-endpoint", "1.0", "Get1:1.0")]
        [InlineData("multiple-endpoints", "2.0", "Get2_0")]
        [InlineData("multiple-endpoints", "2.1", "Get2_1")]
        [InlineData("multiple-versions", "3.0", "Get3:3.0")]
        [InlineData("multiple-versions", "3.1", "Get3:3.1")]
        public async Task ShouldInvokeCorrectEndpoint(string path, string apiVersion, string responseContent)
        {
            var client = await CreateTestClient();

            var response = await client.GetAsync($"api-versioning/{path}?api-version={apiVersion}");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).ShouldBe(responseContent);
        }

        [Theory]
        [InlineData("single-endpoint", "Get1:1.0")]
        [InlineData("multiple-endpoints", "Get2_0")]
        public async Task ShouldSupportDefaultVersion(string path, string responseContent)
        {
            var client = await CreateTestClient();

            var response = await client.GetAsync($"api-versioning/{path}");

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await response.Content.ReadAsStringAsync()).ShouldBe(responseContent);
        }

        [Theory]
        [InlineData("single-endpoint", "api-version=x", "Supported api version is 1.0.")]
        [InlineData("multiple-endpoints", "api-version=x", "Supported api versions are 2.0, 2.1.")]
        [InlineData("multiple-versions", "", "Supported api versions are 3.0, 3.1.")]
        [InlineData("multiple-versions", "api-version=x", "Supported api versions are 3.0, 3.1.")]
        public async Task ShouldHandleUnsupportedApiVersion(string path, string query, string errorDetails)
        {
            var client = await CreateTestClient();

            var response = await client.GetAsync($"api-versioning/{path}?{query}");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
            response.Content.Headers.ContentType.CharSet.ShouldBe("utf-8");
            response.Content.Headers.ContentType.MediaType.ShouldBe("application/problem+json");

            var json = await response.Content.ReadAsStringAsync();
            var details = JsonConvert.DeserializeObject<ProblemDetails>(json);

            details.Detail.ShouldBe(errorDetails);
            details.Status.ShouldBe(StatusCodes.Status404NotFound);
            details.Type.ShouldBe("https://httpstatuses.com/404");
        }

        private static async Task<HttpClient> CreateTestClient()
        {
            var builder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                });

            var host = await builder.StartAsync();

            var client = host.GetTestClient();
            return client;
        }

        public class Startup
        {
            public void ConfigureServices(IServiceCollection services)
            {
                services.AddApiVersioning();
                services.AddControllers();
            }

            public void Configure(IApplicationBuilder app)
            {
                app.UseRouting();
                app.UseEndpoints(endpoints => endpoints.MapControllers());
            }
        }

        [HttpGet("single-endpoint"), ApiVersion("1.0", DefaultVersion = "1.0", Optional = true)]
        public string Get1(string apiVersion)
        {
            return $"Get1:{apiVersion}";
        }

        [HttpGet("multiple-endpoints"), ApiVersion("2.0", DefaultVersion = "2.0", Optional = true)]
        public string Get2_0()
        {
            return "Get2_0";
        }

        [HttpGet("multiple-endpoints"), ApiVersion("2.1")]
        public string Get2_1()
        {
            return "Get2_1";
        }

        [HttpGet("multiple-versions"), ApiVersion(new[] { "3.0", "3.1" })]
        public string Get3(string apiVersion)
        {
            return $"Get3:{apiVersion}";
        }
    }
}