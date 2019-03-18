using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning
{
    public interface IApiVersionValidator
    {
        bool Validate(string version, bool optional, StringValues validVersions, out string matchedVersion,
            out string customProblemDetail);
    }
}