using Handyman.AspNetCore.ApiVersioning.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ApiVersioning.Routing
{
    internal class ApiVersionEndpointMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();
        private static readonly RouteData EmptyRouteData = new RouteData();

        private readonly IApiVersionReader _apiVersionReader;
        private readonly IApiVersionParser _apiVersionParser;

        public ApiVersionEndpointMatcherPolicy(IApiVersionReader apiVersionReader, IApiVersionParser apiVersionParser)
        {
            _apiVersionReader = apiVersionReader;
            _apiVersionParser = apiVersionParser;
        }

        public override int Order { get; } = 0;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            for (var i = 0; i < endpoints.Count; i++)
            {
                var descriptor = endpoints[i].Metadata.GetMetadata<ApiVersionDescriptor>();

                if (descriptor != null)
                {
                    return true;
                }
            }

            return false;
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            IApiVersion requestedApiVersion = null;

            if (_apiVersionReader.TryRead(httpContext.Request, out var values))
            {
                if (!_apiVersionParser.TryParse(values, out requestedApiVersion))
                {
                    WriteErrorResponse(httpContext, candidates);
                    return Task.CompletedTask;
                }
            }

            var endpointFound = false;

            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];

                var descriptor = candidate.Endpoint.Metadata.GetMetadata<ApiVersionDescriptor>();

                if (requestedApiVersion == null)
                {
                    if (descriptor == null)
                    {
                        endpointFound = true;
                        candidates.SetValidity(i, true);
                    }
                    else if (descriptor.IsOptional)
                    {
                        endpointFound = true;
                        candidates.SetValidity(i, true);

                        if (descriptor.DefaultVersion != null)
                        {
                            AddApiVersionFeature(httpContext, descriptor.DefaultVersion.Text);
                        }
                    }

                    continue;
                }

                if (descriptor == null)
                    continue;

                foreach (var version in descriptor.Versions)
                {
                    if (!version.IsMatch(requestedApiVersion))
                        continue;

                    endpointFound = true;
                    candidates.SetValidity(i, true);
                    AddApiVersionFeature(httpContext, version.Text);
                    break;
                }
            }

            if (!endpointFound)
            {
                WriteErrorResponse(httpContext, candidates);
            }

            return Task.CompletedTask;
        }

        private static void WriteErrorResponse(HttpContext httpContext, CandidateSet candidates)
        {
            var actionResultExecutor = httpContext.RequestServices.GetRequiredService<IActionResultExecutor<ObjectResult>>();

            var routeData = httpContext.GetRouteData() ?? EmptyRouteData;
            var actionContext = new ActionContext(httpContext, routeData, EmptyActionDescriptor);

            var problemDetails = GetProblemDetails(candidates);
            var result = new ObjectResult(problemDetails) { StatusCode = 400 };

            var requestDelegate = new RequestDelegate(ctx => actionResultExecutor.ExecuteAsync(actionContext, result));

            var endpoint = new Endpoint(requestDelegate, EndpointMetadataCollection.Empty, default);

            httpContext.SetEndpoint(endpoint);
        }

        private static ProblemDetails GetProblemDetails(CandidateSet candidates)
        {
            var versions = new List<string>();

            for (var i = 0; i < candidates.Count; i++)
            {
                var descriptor = candidates[i].Endpoint.Metadata.GetMetadata<ApiVersionDescriptor>();

                if (descriptor == null)
                    continue;

                foreach (var version in descriptor.Versions)
                {
                    versions.Add(version.Text);
                }
            }

            var detail = versions.Count == 0
                ? "Invalid api version" // this should never happen but just in case...
                : versions.Count == 1
                    ? $"Invalid api version, supported version is {versions[0]}"
                    : $"Invalid api version, supported versions are {string.Join(", ", versions)}";

            return new ProblemDetails
            {
                Detail = detail,
                Status = 400,
                Title = ReasonPhrases.GetReasonPhrase(400),
                Type = "https://httpstatuses.com/400"
            };
        }

        private static void AddApiVersionFeature(HttpContext httpContext, string version)
        {
            httpContext.Features.Set(new ApiVersionFeature { MatchedVersion = version });
        }
    }
}