using Handyman.AspNetCore.ApiVersioning.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning;

public static class RouteHandlerBuilderExtensions
{
#if NET7_0_OR_GREATER

    public static RouteHandlerBuilder ApiVersion(this RouteHandlerBuilder builder, string version, string defaultVersion = null, bool? optional = null)
    {
        return builder.ApiVersion(new[] { version }, defaultVersion, optional);
    }

    public static RouteHandlerBuilder ApiVersion(this RouteHandlerBuilder builder, IEnumerable<string> versions, string defaultVersion = null, bool? optional = null)
    {
        builder.Add(endpointBuilder =>
        {
            var provider = endpointBuilder.ApplicationServices.GetRequiredService<IApiVersionDescriptorProvider>();

            var apiVersionDescriptor = provider.GetApiVersionDescriptor(endpointBuilder.DisplayName, versions.ToList(), defaultVersion, optional);

            endpointBuilder.Metadata.Add(apiVersionDescriptor);
        });

        return builder;
    }

#endif

    private static IApiVersion Parse(string version, IApiVersionParser parser)
    {
        return parser.TryParse(version, out var apiVersion) ? apiVersion : throw new Exception("Invalid api version format.");
    }
}