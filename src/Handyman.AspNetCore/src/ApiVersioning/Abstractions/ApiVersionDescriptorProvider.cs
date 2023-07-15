using Microsoft.AspNetCore.Mvc.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning.Abstractions;

internal class ApiVersionDescriptorProvider : IActionDescriptorProvider, IApiVersionDescriptorProvider
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
            action.EndpointMetadata.Add(apiVersionDescriptor);
        }
    }

    private ApiVersionDescriptor GetApiVersionDescriptor(ActionDescriptor action, ApiVersionAttribute apiVersionAttribute)
    {
        return GetApiVersionDescriptor(action.DisplayName, apiVersionAttribute.Versions, apiVersionAttribute.DefaultVersion, apiVersionAttribute.NullableOptional);
    }

    public void OnProvidersExecuted(ActionDescriptorProviderContext context)
    {
    }

    public ApiVersionDescriptor GetApiVersionDescriptor(string endpointDisplayName, IReadOnlyCollection<string> versions, string nullableDefaultVersion, bool? isOptional)
    {
        if (versions.Count == 0)
        {
            throw new InvalidOperationException($"{endpointDisplayName} : does not have any supported versions defined.");
        }

        var defaultApiVersion = !string.IsNullOrWhiteSpace(nullableDefaultVersion)
            ? ParseApiVersion(nullableDefaultVersion, endpointDisplayName)
            : null;
        var apiVersions = versions
            .Select(x => ParseApiVersion(x, endpointDisplayName))
            .ToArray();

        if (defaultApiVersion != null)
        {
            if (apiVersions.All(x => x.Text != defaultApiVersion.Text))
            {
                throw new InvalidOperationException($"{endpointDisplayName} : default version does not match any of the supported versions.");
            }
        }

        return new ApiVersionDescriptor
        {
            DefaultVersion = defaultApiVersion,
            IsOptional = isOptional ?? nullableDefaultVersion is not null,
            Versions = apiVersions
        };
    }

    private IApiVersion ParseApiVersion(string version, string endpointDisplayName)
    {
        if (_apiVersionParser.TryParse(version, out var apiVersion))
            return apiVersion;

        throw new FormatException($"{endpointDisplayName} : version '{version}' has an invalid format.");
    }
}