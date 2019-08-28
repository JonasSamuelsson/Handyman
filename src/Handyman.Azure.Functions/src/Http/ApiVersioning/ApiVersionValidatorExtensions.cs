using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
   public static class ApiVersionValidatorExtensions
   {
      public static bool Validate(this IApiVersionValidator validator, HttpRequest request, bool optional, StringValues validVersions, out Result result)
      {
         var isValid = validator.Validate(request, optional, validVersions, out var matchedVersion, out var error);
         result = CreateResult(isValid, matchedVersion, error);
         return isValid;
      }

      public static bool Validate(this IApiVersionValidator validator, HttpRequest request, out string matchedVersion, out string error)
      {
         var stringValues = request.Headers["api-versioning"];

         if (StringValues.IsNullOrEmpty(stringValues))
         {
            var message = "In order to use this extension method the function must be decorated with ApiVersionAttribute.";
            throw new InvalidOperationException(message);
         }

         var optional = stringValues[0] == "true";
         var validVersions = stringValues.Skip(1).ToArray();

         return validator.Validate(request, optional, validVersions, out matchedVersion, out error);
      }

      public static bool Validate(this IApiVersionValidator validator, HttpRequest request, out Result result)
      {
         var isValid = validator.Validate(request, out var matchedVersion, out var error);
         result = CreateResult(isValid, matchedVersion, error);
         return isValid;
      }

      private static Result CreateResult(bool isValid, string matchedVersion, string error)
      {
         return isValid
            ? new Result { MatchedVersion = matchedVersion }
            : new Result { ProblemDetails = new ProblemDetails(HttpStatusCode.BadRequest, error) };
      }

      public class Result
      {
         public string MatchedVersion { get; set; }
         public ProblemDetails ProblemDetails { get; set; }
      }
   }
}