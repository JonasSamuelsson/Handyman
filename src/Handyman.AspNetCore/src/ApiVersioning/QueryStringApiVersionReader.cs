using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal class QueryStringApiVersionReader : IApiVersionReader
    {
        public StringValues Read(HttpRequest request)
        {
            return request.Query["api-version"];
        }
    }
}