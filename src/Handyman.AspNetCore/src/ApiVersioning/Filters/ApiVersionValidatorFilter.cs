using Handyman.AspNetCore.ApiVersioning.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Handyman.AspNetCore.ApiVersioning.Filters
{
    internal class ApiVersionValidatorFilter : IResourceFilter
    {
        private readonly IApiVersionReader _apiVersionReader;
        private readonly IApiVersionParser _apiVersionParser;

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

            if (!_apiVersionReader.TryReadApiVersion(context.HttpContext.Request, out var rawApiVersion))
            {
                if (descriptor.IsOptional)
                {
                    var defaultVersion = descriptor.DefaultVersion;

                    if (!string.IsNullOrWhiteSpace(defaultVersion))
                    {
                        AddApiVersionFeature(context.HttpContext, defaultVersion);
                    }

                    return;
                }

                SetBadRequestResponse(context, descriptor.ErrorMessage);
                return;
            }

            if (!_apiVersionParser.TryParse(rawApiVersion, out var apiVersion))
            {
                SetBadRequestResponse(context, descriptor.ErrorMessage);
                return;
            }

            foreach (var version in descriptor.Versions)
            {
                if (!version.IsMatch(apiVersion))
                    continue;

                AddApiVersionFeature(context.HttpContext, version.Text);
                return;
            }

            SetBadRequestResponse(context, descriptor.ErrorMessage);
        }

        private static void AddApiVersionFeature(HttpContext httpContext, string apiVersion)
        {
            httpContext.Features.Set(new ApiVersionFeature { Version = apiVersion });
        }

        private static void SetBadRequestResponse(ResourceExecutingContext context, string errorMessage)
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