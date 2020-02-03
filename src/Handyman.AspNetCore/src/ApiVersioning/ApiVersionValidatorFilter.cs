using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Concurrent;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal class ApiVersionValidatorFilter : IResourceFilter
    {
        private readonly ApiVersionDescriptorProvider _apiVersionDescriptorProvider;
        private readonly IApiVersionReader _apiVersionReader;
        private readonly IApiVersionParser _apiVersionParser;
        private readonly ConcurrentDictionary<string, IApiVersion> _apiVersionCache;

        public ApiVersionValidatorFilter(ApiVersionDescriptorProvider apiVersionDescriptorProvider, IApiVersionReader apiVersionReader, IApiVersionParser apiVersionParser)
        {
            _apiVersionDescriptorProvider = apiVersionDescriptorProvider;
            _apiVersionReader = apiVersionReader;
            _apiVersionParser = apiVersionParser;
            _apiVersionCache = new ConcurrentDictionary<string, IApiVersion>();
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var descriptor = _apiVersionDescriptorProvider.GetApiVersionDescriptor(context.ActionDescriptor);

            if (descriptor.Versions.Length == 0)
                return;

            if (!_apiVersionReader.TryReadApiVersion(context.HttpContext.Request, out var rawApiVersion))
            {
                if (descriptor.Optional)
                {
                    var defaultVersion = descriptor.DefaultVersion;

                    if (defaultVersion != null)
                    {
                        AddApiVersionFeature(context.HttpContext, defaultVersion);
                    }

                    return;
                }

                SetBadRequestResponse(context, descriptor.ErrorMessage);
                return;
            }

            if (!_apiVersionCache.TryGetValue(rawApiVersion, out var apiVersion) && !_apiVersionParser.TryParse(rawApiVersion, out apiVersion))
            {
                SetBadRequestResponse(context, descriptor.ErrorMessage);
                return;
            }

            foreach (var descriptorVersion in descriptor.Versions)
            {
                if (!descriptorVersion.IsMatch(apiVersion))
                    continue;

                _apiVersionCache.TryAdd(rawApiVersion, apiVersion);
                AddApiVersionFeature(context.HttpContext, descriptorVersion.Text);
                return;
            }

            SetBadRequestResponse(context, descriptor.ErrorMessage);
        }

        private void AddApiVersionFeature(HttpContext httpContext, string apiVersion)
        {
            httpContext.Features.Set(new ApiVersionFeature { Version = apiVersion });
        }

        private void SetBadRequestResponse(ResourceExecutingContext context, string errorMessage)
        {
            var problemDetails = new ProblemDetails
            {
                Detail = errorMessage,
                Status = 400,
                Title = "Bad request, invalid api version.",
                Type = "https://httpstatuses.com/400"
            };

            context.Result = new ObjectResult(problemDetails) { StatusCode = 400 };
        }
    }
}