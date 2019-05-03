using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning
{
    public interface IApiVersionReader
    {
        StringValues Read(HttpRequest request);
    }
}