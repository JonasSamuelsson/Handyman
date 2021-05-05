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

namespace Handyman.AspNetCore.ApiVersioning.Internals.Routing
{
    internal class ApiVersionEndpointMatcherPolicy : MatcherPolicy, IEndpointSelectorPolicy
    {
        private static readonly ActionDescriptor EmptyActionDescriptor = new ActionDescriptor();
        private static readonly RouteData EmptyRouteData = new RouteData();

        private readonly IEnumerable<IApiVersionReader> _apiVersionReaders;
        private readonly IApiVersionParser _apiVersionParser;

        public ApiVersionEndpointMatcherPolicy(IEnumerable<IApiVersionReader> apiVersionReaders, IApiVersionParser apiVersionParser)
        {
            _apiVersionReaders = apiVersionReaders;
            _apiVersionParser = apiVersionParser;
        }

        public override int Order { get; } = 0;

        public bool AppliesToEndpoints(IReadOnlyList<Endpoint> endpoints)
        {
            for (var i = 0; i < endpoints.Count; i++)
            {
                var descriptor = endpoints[i].Metadata.GetMetadata<ApiVersionMetadata>();

                if (descriptor != null)
                {
                    return true;
                }
            }

            return false;
        }

        public Task ApplyAsync(HttpContext httpContext, CandidateSet candidates)
        {
            string requestString = null;

            foreach (var apiVersionReader in _apiVersionReaders)
            {
                if (!apiVersionReader.TryRead(httpContext.Request, out var value))
                    continue;

                if (requestString != null)
                {
                    WriteErrorResponse(httpContext, candidates);
                    return Task.CompletedTask;
                }

                requestString = value;
            }

            if (!_apiVersionParser.TryParse(requestString, out var requestApiVersion))
            {
                WriteErrorResponse(httpContext, candidates);
                return Task.CompletedTask;
            }

            var endpointFound = false;

            for (var i = 0; i < candidates.Count; i++)
            {
                var candidate = candidates[i];

                var apiVersionMetadata = candidate.Endpoint.Metadata.GetMetadata<ApiVersionMetadata>();

                if (requestApiVersion == null)
                {
                    if (apiVersionMetadata == null)
                    {
                        endpointFound = true;
                        candidates.SetValidity(i, true);
                    }
                    else if (apiVersionMetadata.IsOptional)
                    {
                        endpointFound = true;
                        candidates.SetValidity(i, true);

                        if (apiVersionMetadata.DefaultVersion != null)
                        {
                            AddApiVersionFeature(httpContext, apiVersionMetadata.DefaultVersion.Text);
                        }
                    }
                    else
                    {
                        candidates.SetValidity(i, false);
                    }

                    continue;
                }

                if (apiVersionMetadata == null)
                    continue;

                foreach (var version in apiVersionMetadata.Versions)
                {
                    if (!version.IsMatch(requestApiVersion))
                    {
                        candidates.SetValidity(i, false);
                        continue;
                    }

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
                var descriptor = candidates[i].Endpoint.Metadata.GetMetadata<ApiVersionMetadata>();

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