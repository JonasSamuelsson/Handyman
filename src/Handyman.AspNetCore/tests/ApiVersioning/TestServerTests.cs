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
using System.Threading.Tasks;
using Xunit;

namespace Handyman.AspNetCore.Tests.ApiVersioning
{
    [ApiController, Route("api-versioning")]
    public class TestServerTests : ControllerBase
    {
        [Theory]
        [InlineData("", "Get0:0.0")]
        [InlineData("api-version=1.0", "Get1:1.0")]
        [InlineData("api-version=2.0", "Get2:2.0")]
        [InlineData("api-version=3.0", null)]
        public async Task ShouldInvokeCorrectEndpoint(string query, string content)
        {
            var builder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.UseStartup<Startup>();
                });

            var host = await builder.StartAsync();

            var client = host.GetTestClient();

            var response = await client.GetAsync($"api-versioning?{query}");

            if (response.IsSuccessStatusCode)
            {
                (await response.Content.ReadAsStringAsync()).ShouldBe(content);
                return;
            }

            response.Content.Headers.ContentType.CharSet.ShouldBe("utf-8");
            response.Content.Headers.ContentType.MediaType.ShouldBe("application/problem+json");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var json = await response.Content.ReadAsStringAsync();
            var details = JsonConvert.DeserializeObject<ProblemDetails>(json);

            details.Detail.ShouldBe("Invalid api version, supported versions are 0.0, 1.0, 2.0");
            details.Status.ShouldBe(StatusCodes.Status400BadRequest);
            details.Title.ShouldBe("Bad Request");
            details.Type.ShouldBe("https://httpstatuses.com/400");
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

        [HttpGet, ApiVersion("0.0", Optional = true, DefaultVersion = "0.0")]
        public string Get0(string apiVersion)
        {
            return $"Get0:{apiVersion}";
        }

        [HttpGet, ApiVersion("1.0")]
        public string Get1(string apiVersion)
        {
            return $"Get1:{apiVersion}";
        }

        [HttpGet, ApiVersion("2.0")]
        public string Get2(string apiVersion)
        {
            return $"Get2:{apiVersion}";
        }
    }
}
