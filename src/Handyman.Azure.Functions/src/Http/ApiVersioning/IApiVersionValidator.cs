using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
   public interface IApiVersionValidator
   {
      bool Validate(HttpRequest request, bool optional, StringValues validVersions, out string matchedVersion, out string error);
   }
}