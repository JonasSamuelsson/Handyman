using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Handyman.AspNetCore.ETags.Internals.Middleware
{
    internal static class ProblemDetailsResultFactory
    {
        public static ObjectResult CreateResult(int statusCode, string details)
        {
            var problemDetails = new
            {
                Details = details,
                Status = statusCode,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Type = $"https://httpstatuses.com/{statusCode}"
            };

            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode
            };
        }
    }
}