using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    internal class QueryStringApiVersionReader : IApiVersionReader
    {
        public StringValues Read(HttpRequest request)
        {
            return request.Query["api-version"];
        }
    }
}