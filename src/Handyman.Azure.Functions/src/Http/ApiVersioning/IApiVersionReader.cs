using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
   public interface IApiVersionReader
   {
      StringValues Read(HttpRequest request);
   }
}