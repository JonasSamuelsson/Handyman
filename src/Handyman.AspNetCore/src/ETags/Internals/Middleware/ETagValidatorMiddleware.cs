using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ETags.Internals.Middleware;

internal class ETagValidatorMiddleware : IMiddleware
{
    private readonly IETagValidator _eTagValidator;
    private readonly ProblemDetailsResponseWriter _problemDetailsResponseWriter;

    public ETagValidatorMiddleware(IETagValidator eTagValidator, ProblemDetailsResponseWriter problemDetailsResponseWriter)
    {
        _eTagValidator = eTagValidator;
        _problemDetailsResponseWriter = problemDetailsResponseWriter;
    }

    public Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var endpoint = context.GetEndpoint();

        if (endpoint != null)
        {
            var headers = context.Request.Headers;

            foreach (var metadata in endpoint.Metadata.GetOrderedMetadata<ETagMetadata>())
            {
                if (Validate(headers, metadata, out var statusCode, out var details))
                    continue;

                return _problemDetailsResponseWriter.WriteResponse(context, statusCode, details);
            }
        }

        return next(context);
    }

    private bool Validate(IHeaderDictionary headers, ETagMetadata metadata, out int statusCode, out string details)
    {
        if (headers.TryGetValue(metadata.HeaderName, out var values))
        {
            if (!_eTagValidator.IsValidETag(values))
            {
                statusCode = StatusCodes.Status400BadRequest;
                details = $"{metadata.HeaderName} header value has an invalid format.";
                return false;
            }
        }
        else if (metadata.IsRequired)
        {
            statusCode = StatusCodes.Status428PreconditionRequired;
            details = $"Missing required {metadata.HeaderName} header.";
            return false;
        }

        statusCode = default;
        details = default;
        return true;
    }
}