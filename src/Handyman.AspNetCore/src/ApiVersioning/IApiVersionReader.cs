using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning
{
    public interface IApiVersionReader
    {
        bool TryReadApiVersion(HttpRequest httpRequest, out StringValues apiVersion);
    }
}