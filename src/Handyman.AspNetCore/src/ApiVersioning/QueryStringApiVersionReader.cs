using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning;

internal class QueryStringApiVersionReader : IApiVersionReader
{
    public bool TryRead(HttpRequest httpRequest, out StringValues values)
    {
        return httpRequest.Query.TryGetValue("api-version", out values);
    }
}