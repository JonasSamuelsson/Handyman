using Handyman.AspNetCore.ApiVersioning.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning.Filters
{
    internal class ApiVersionValidatorFilter : IResourceFilter
    {
        private readonly IApiVersionReader _apiVersionReader;
        private readonly IApiVersionParser _apiVersionParser;
        private readonly ConcurrentDictionary<ApiVersionDescriptor, string> _errorMessages = new ConcurrentDictionary<ApiVersionDescriptor, string>();

        public ApiVersionValidatorFilter(IApiVersionReader apiVersionReader, IApiVersionParser apiVersionParser)
        {
            _apiVersionReader = apiVersionReader;
            _apiVersionParser = apiVersionParser;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var descriptor = context.ActionDescriptor.GetProperty<ApiVersionDescriptor>();

            IApiVersion requestedApiVersion = null;

            if (_apiVersionReader.TryRead(context.HttpContext.Request, out var values))
            {
                if (!_apiVersionParser.TryParse(values, out requestedApiVersion))
                {
                    WriteErrorResponse(context, descriptor);
                    return;
                }
            }

            if (requestedApiVersion == null)
            {
                if (descriptor.IsOptional)
                {
                    if (descriptor.DefaultVersion != null)
                    {
                        AddApiVersionFeature(context.HttpContext, descriptor.DefaultVersion.Text);
                    }

                    return;
                }
            }

            foreach (var version in descriptor.Versions)
            {
                if (!version.IsMatch(requestedApiVersion))
                    continue;

                AddApiVersionFeature(context.HttpContext, version.Text);
                return;
            }

            WriteErrorResponse(context, descriptor);
        }

        private static void AddApiVersionFeature(HttpContext httpContext, string apiVersion)
        {
            httpContext.Features.Set(new ApiVersionFeature { MatchedVersion = apiVersion });
        }

        private void WriteErrorResponse(ResourceExecutingContext context, ApiVersionDescriptor descriptor)
        {
            var problemDetails = new ProblemDetails
            {
                Detail = _errorMessages.GetOrAdd(descriptor, x => GetErrorMessage(x)),
                Status = 400,
                Title = "Bad request, invalid api version.",
                Type = "https://httpstatuses.com/400"
            };

            context.Result = new ObjectResult(problemDetails) { StatusCode = 400 };
        }

        private string GetErrorMessage(ApiVersionDescriptor descriptor)
        {
            return descriptor.Versions.Length == 1
                ? $"Invalid api version, supported version is {descriptor.Versions[0].Text}."
                : $"Invalid api version, supported versions are {string.Join(", ", descriptor.Versions.Select(x => x.Text))}.";
        }
    }
}