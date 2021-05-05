using Microsoft.AspNetCore.Mvc.Abstractions;
using System;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning.Internals.AppModel
{
    internal class ApiVersionActionDescriptorProvider : IActionDescriptorProvider
    {
        private readonly IApiVersionParser _apiVersionParser;

        public ApiVersionActionDescriptorProvider(IApiVersionParser apiVersionParser)
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
                var apiVersionMetadata = GetApiVersionMetadata(action, attribute);
                action.EndpointMetadata.Add(apiVersionMetadata);
            }
        }

        private ApiVersionMetadata GetApiVersionMetadata(ActionDescriptor action, ApiVersionAttribute apiVersionAttribute)
        {
            if (apiVersionAttribute.Versions.Count == 0)
                throw new InvalidOperationException($"{action.DisplayName} : does not have any supported versions.");

            var defaultApiVersion = !string.IsNullOrWhiteSpace(apiVersionAttribute.DefaultVersion)
                ? ParseApiVersion(apiVersionAttribute.DefaultVersion, action)
                : null;

            var apiVersions = apiVersionAttribute.Versions
                .Select(x => ParseApiVersion(x, action))
                .ToArray();

            if (defaultApiVersion != null)
            {
                if (apiVersions.All(x => x.Text != defaultApiVersion.Text))
                {
                    throw new InvalidOperationException($"{action.DisplayName} : default version does not match any of the supported versions.");
                }
            }

            return new ApiVersionMetadata
            {
                DefaultVersion = defaultApiVersion,
                IsOptional = apiVersionAttribute.Optional,
                Versions = apiVersions
            };
        }

        private IApiVersion ParseApiVersion(string version, ActionDescriptor action)
        {
            if (_apiVersionParser.TryParse(version, out var apiVersion))
                return apiVersion;

            throw new FormatException($"{action.DisplayName} : api version '{version}' has an invalid format.");
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }
    }
}