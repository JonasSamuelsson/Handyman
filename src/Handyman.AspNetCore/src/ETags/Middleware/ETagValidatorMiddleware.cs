using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ETags.Middleware
{
    internal class ETagValidatorMiddleware : IMiddleware
    {
        private const string MissingRequiredIfMatchHeader = "Required header If-Match is missing.";
        private const string InvalidIfMatchHeaderFormat = "E-tag in If-Match header has an invalid format.";
        private const string InvalidIfNoneMatchHeaderFormat = "E-tag in If-None-Match header has an invalid format.";

        private readonly IETagValidator _eTagValidator;
        private readonly ProblemDetailsResponseWriter _problemDetailsResponseWriter;

        public ETagValidatorMiddleware(IETagValidator eTagValidator, ProblemDetailsResponseWriter problemDetailsResponseWriter)
        {
            _eTagValidator = eTagValidator;
            _problemDetailsResponseWriter = problemDetailsResponseWriter;
        }

        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var headers = context.Request.Headers;

            if (context.GetEndpoint()?.Metadata.GetMetadata<EnsureRequestHasIfMatchHeader>() != null)
            {
                if (!headers.TryGetValue(HeaderNames.IfMatch, out var ifMatch))
                {
                    return _problemDetailsResponseWriter.WriteResponse(context, StatusCodes.Status428PreconditionRequired, MissingRequiredIfMatchHeader);
                }

                if (!_eTagValidator.IsValidETag(ifMatch))
                {
                    return _problemDetailsResponseWriter.WriteResponse(context, StatusCodes.Status400BadRequest, InvalidIfMatchHeaderFormat);
                }
            }

            if (headers.TryGetValue(HeaderNames.IfNoneMatch, out var ifNoneMatch) && !_eTagValidator.IsValidETag(ifNoneMatch))
            {
                return _problemDetailsResponseWriter.WriteResponse(context, StatusCodes.Status400BadRequest, InvalidIfNoneMatchHeaderFormat);
            }

            return next(context);
        }
    }
}