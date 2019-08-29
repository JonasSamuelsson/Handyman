using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal class ApiVersionValidationFilter : IAsyncActionFilter
    {
        private readonly StringValues _validVersions;
        private readonly bool _optional;
        private readonly string _defaultVersion;
        private readonly IApiVersionReader _apiVersionReader;
        private readonly IApiVersionValidator _apiVersionValidator;

        public ApiVersionValidationFilter(StringValues validVersions, bool optional, string defaultVersion,
            IApiVersionReader apiVersionReader, IApiVersionValidator apiVersionValidator)
        {
            _validVersions = validVersions;
            _optional = optional;
            _defaultVersion = defaultVersion;
            _apiVersionReader = apiVersionReader;
            _apiVersionValidator = apiVersionValidator;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var version = _apiVersionReader.Read(context.HttpContext.Request);

            if (_apiVersionValidator.Validate(version, _optional, _validVersions, out var matchedVersion, out var detail))
            {
                PopulateActionParameter(context, matchedVersion ?? _defaultVersion);
                return next();
            }

            var details = new ProblemDetails
            {
                Detail = detail ?? $"Invalid api version, supported versions: {string.Join(", ", _validVersions)}",
                Status = 400,
                Title = "Bad request, invalid api version.",
                Type = "https://httpstatuses.com/400"
            };

            context.Result = new BadRequestObjectResult(details);

            return Task.CompletedTask;
        }

        private static void PopulateActionParameter(ActionExecutingContext context, string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return;

            var parameters = context.ActionDescriptor.Parameters;

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];

                if (parameter.ParameterType != typeof(string))
                    continue;

                var name = parameter.Name;

                if (name.Equals("apiversion", StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                context.ActionArguments[name] = version;
                return;
            }
        }
    }
}