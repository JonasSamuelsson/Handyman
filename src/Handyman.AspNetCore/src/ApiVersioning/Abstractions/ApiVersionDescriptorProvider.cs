using Handyman.AspNetCore.ApiVersioning.Filters;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning.Abstractions
{
    internal class ApiVersionDescriptorProvider : IActionDescriptorProvider
    {
        private readonly IApiVersionParser _apiVersionParser;

        public ApiVersionDescriptorProvider(IApiVersionParser apiVersionParser)
        {
            _apiVersionParser = apiVersionParser;
        }

        public int Order { get; } = 0;

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            foreach (var action in context.Results)
            {
                var filterDescriptor = action.FilterDescriptors.SingleOrDefault(x => x.Filter is ApiVersionAttribute);

                if (filterDescriptor == null)
                    continue;

                var attribute = (ApiVersionAttribute)filterDescriptor.Filter;
                var apiVersionDescriptor = GetApiVersionDescriptor(action, attribute);

#if NETSTANDARD2_0
                action.FilterDescriptors.Add(new FilterDescriptor(new ApiVersionValidatorFilterFactory(), filterDescriptor.Scope));
                action.SetProperty(apiVersionDescriptor);
#else
                action.EndpointMetadata.Add(apiVersionDescriptor);
#endif

                foreach (var parameterDescriptor in action.Parameters)
                {
                    if (!parameterDescriptor.Name.Equals("apiVersion", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var apiVersionParameterBinderFilter = new ApiVersionParameterBindingFilter(parameterDescriptor.Name);
                    action.FilterDescriptors.Add(new FilterDescriptor(apiVersionParameterBinderFilter, filterDescriptor.Scope));
                }
            }
        }

        private ApiVersionDescriptor GetApiVersionDescriptor(ActionDescriptor action, ApiVersionAttribute apiVersionAttribute)
        {
            if (apiVersionAttribute.Versions.Count == 0)
                throw new InvalidOperationException($"{action.DisplayName} : does not have any supported versions.");

            var defaultApiVersion = string.IsNullOrWhiteSpace(apiVersionAttribute.DefaultVersion)
                ? ParseApiVersion(action, apiVersionAttribute.DefaultVersion)
                : null;

            var apiVersions = apiVersionAttribute.Versions
                .Select(x => ParseApiVersion(action, x))
                .OrderBy(x => x)
                .ToArray();

            if (defaultApiVersion != null)
            {
                foreach (var version in apiVersions)
                {
                    if (defaultApiVersion.Text == version.Text)
                        continue;

                    throw new InvalidOperationException($"{action.DisplayName} : default version does not match any of the supported versions.");
                }
            }

            return new ApiVersionDescriptor
            {
                DefaultVersion = defaultApiVersion,
                IsOptional = apiVersionAttribute.Optional,
                Versions = apiVersions
            };
        }

        private IApiVersion ParseApiVersion(ActionDescriptor action, string s)
        {
            if (_apiVersionParser.TryParse(s, out var apiVersion))
                return apiVersion;

            throw new FormatException($"{action.DisplayName} : version '{s}' has an invalid format.");
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }
    }
}