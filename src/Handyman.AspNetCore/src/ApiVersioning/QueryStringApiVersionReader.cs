using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal class QueryStringApiVersionReader : IApiVersionReader
    {
        public bool TryReadApiVersion(HttpRequest httpRequest, out StringValues apiVersion)
        {
            return httpRequest.Query.TryGetValue("api-version", out apiVersion);
        }
    }
}