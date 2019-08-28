using Microsoft.Extensions.Primitives;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
   public interface IApiVersionValidationStrategy
   {
      bool Validate(string version, bool optional, StringValues validVersions, out string matchedVersion, out string error);
   }
}