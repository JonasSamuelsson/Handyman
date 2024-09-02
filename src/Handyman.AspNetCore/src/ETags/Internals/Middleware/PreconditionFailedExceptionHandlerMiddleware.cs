using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ETags.Internals.Middleware;

internal class PreconditionFailedExceptionHandlerMiddleware : IMiddleware
{
    private const int StatusCode = StatusCodes.Status412PreconditionFailed;
    private static readonly string Details = ReasonPhrases.GetReasonPhrase(StatusCode);

    private readonly ProblemDetailsResponseWriter _problemDetailsResponseWriter;

    public PreconditionFailedExceptionHandlerMiddleware(ProblemDetailsResponseWriter problemDetailsResponseWriter)
    {
        _problemDetailsResponseWriter = problemDetailsResponseWriter;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context).ConfigureAwait(false);
        }
        catch (PreconditionFailedException)
        {
            await _problemDetailsResponseWriter.WriteResponse(context, StatusCode, Details).ConfigureAwait(false);
        }
    }
}