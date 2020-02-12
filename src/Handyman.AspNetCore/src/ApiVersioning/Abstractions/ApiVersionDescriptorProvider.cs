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

        public int Order { get; }

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            foreach (var action in context.Results)
            {
                var filterDescriptor = action.FilterDescriptors.SingleOrDefault(x => x.Filter is ApiVersionAttribute);

                if (filterDescriptor == null)
                    continue;

                var attribute = (ApiVersionAttribute)filterDescriptor.Filter;

                if (attribute.Versions.Count == 0)
                    throw new InvalidOperationException($"{action.DisplayName} does not have any declared api versions.");

                var apiVersionDescriptor = CreateApiVersionDescriptor(attribute);

                action.SetProperty(apiVersionDescriptor);

#if NETSTANDARD2_0
                action.FilterDescriptors.Add(new FilterDescriptor(new ApiVersionValidatorFilterFactory(), filterDescriptor.Scope));
#endif

                foreach (var parameterDescriptor in action.Parameters)
                {
                    if (!parameterDescriptor.Name.Equals("apiVersion", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var apiVersionParameterBinderFilter = new ApiVersionParameterBinderFilter(parameterDescriptor.Name);
                    action.FilterDescriptors.Add(new FilterDescriptor(apiVersionParameterBinderFilter, filterDescriptor.Scope));
                }
            }
        }

        private ApiVersionDescriptor CreateApiVersionDescriptor(ApiVersionAttribute attribute)
        {
            var versions = attribute.Versions
                .Select(ParseApiVersion)
                .OrderBy(x => x)
                .ToArray();

            return new ApiVersionDescriptor
            {
                DefaultVersion = attribute.DefaultVersion,
                ErrorMessage = $"Invalid api version, supported versions are {string.Join(", ", versions.Select(x => x.Text))}.",
                IsOptional = attribute.Optional,
                Versions = versions
            };
        }

        private IApiVersion ParseApiVersion(string s)
        {
            return _apiVersionParser.TryParse(s, out var version)
                ? version
                : throw new FormatException("Invalid api version format.");
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }
    }
}