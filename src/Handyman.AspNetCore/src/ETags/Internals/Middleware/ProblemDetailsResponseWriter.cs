using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ETags.Internals.Middleware;

internal class ProblemDetailsResponseWriter
{
    private static readonly ISet<string> AllowedHeaderNames = GetAllowedHeaderNames();
    private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();
    private static readonly RouteData EmptyRouteData = new RouteData();

    private readonly IActionResultExecutor<ObjectResult> _actionResultExecutor;

    public ProblemDetailsResponseWriter(IActionResultExecutor<ObjectResult> actionResultExecutor)
    {
        _actionResultExecutor = actionResultExecutor;
    }

    internal Task WriteResponse(HttpContext context, int statusCode, string details)
    {
        ClearResponse(context);

        context.Response.StatusCode = statusCode;

        var routeData = context.GetRouteData() ?? EmptyRouteData;
        var actionContext = new ActionContext(context, routeData, EmptyActionDescriptor);
        var result = ProblemDetailsResultFactory.CreateResult(statusCode, details);

        return _actionResultExecutor.ExecuteAsync(actionContext, result);
    }

    private static void ClearResponse(HttpContext context)
    {
        var headers = new HeaderDictionary();

        // Make sure problem responses are never cached.
        headers.Append(HeaderNames.CacheControl, "no-cache, no-store, must-revalidate");
        headers.Append(HeaderNames.Pragma, "no-cache");
        headers.Append(HeaderNames.Expires, "0");

        foreach (var header in context.Response.Headers)
        {
            // Because the CORS middleware adds all the headers early in the pipeline,
            // we want to copy over the existing Access-Control-* headers after resetting the response.
            if (AllowedHeaderNames.Contains(header.Key))
            {
                headers.Add(header);
            }
        }

        context.Response.Clear();

        foreach (var header in headers)
        {
            context.Response.Headers.Add(header);
        }
    }

    private static ISet<string> GetAllowedHeaderNames()
    {
        return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            HeaderNames.AccessControlAllowCredentials,
            HeaderNames.AccessControlAllowHeaders,
            HeaderNames.AccessControlAllowMethods,
            HeaderNames.AccessControlAllowOrigin,
            HeaderNames.AccessControlExposeHeaders,
            HeaderNames.AccessControlMaxAge,
            HeaderNames.AccessControlRequestHeaders,
            HeaderNames.AccessControlRequestMethod,
            HeaderNames.StrictTransportSecurity,
            HeaderNames.WWWAuthenticate
        };
    }
}