using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ApiVersionAttribute : FunctionInvocationFilterAttribute
    {
        private readonly StringValues _validVersions;

        public ApiVersionAttribute(string version)
            : this(new[] {version})
        {
        }

        public ApiVersionAttribute(string[] versions)
        {
            _validVersions = versions;
        }

        public string DefaultVersion { get; set; }
        public bool Optional { get; set; }

        public override Task OnExecutingAsync(FunctionExecutingContext executingContext,
            CancellationToken cancellationToken)
        {
            var request = executingContext.Arguments.Values.OfType<HttpRequest>().First();

            request.Headers[HeaderNames.DefaultVersion] = DefaultVersion;
            request.Headers[HeaderNames.Optional] = Optional.ToString();
            request.Headers[HeaderNames.ValidVersions] = _validVersions;

            return Task.CompletedTask;
        }
    }
}