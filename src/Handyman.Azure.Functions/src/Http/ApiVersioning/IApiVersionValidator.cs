using Microsoft.AspNetCore.Http;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    public interface IApiVersionValidator
    {
        bool Validate(HttpRequest request, out ValidationResult result);
    }
}