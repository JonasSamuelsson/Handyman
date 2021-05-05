using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning
{
    public interface IApiVersionReader
    {
        bool TryRead(HttpRequest httpRequest, out StringValues values);
        ApiVersionSource GetApiVersionSourceOrNull();
    }
}