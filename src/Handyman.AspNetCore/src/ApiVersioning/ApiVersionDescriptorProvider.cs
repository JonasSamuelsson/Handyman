using Microsoft.AspNetCore.Mvc.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal class ApiVersionDescriptorProvider
    {
        private readonly ConcurrentDictionary<ActionDescriptor, ApiVersionDescriptor> _cache;
        private readonly IApiVersionParser _parser;

        public ApiVersionDescriptorProvider(IApiVersionParser parser)
        {
            _cache = new ConcurrentDictionary<ActionDescriptor, ApiVersionDescriptor>();
            _parser = parser;
        }

        internal ApiVersionDescriptor GetApiVersionDescriptor(ActionDescriptor action)
        {
            return _cache.GetOrAdd(action, GetDescriptor);
        }

        private ApiVersionDescriptor GetDescriptor(ActionDescriptor action)
        {
            var attribute = action.FilterDescriptors
                .Select(x => x.Filter)
                .OfType<ApiVersionAttribute>()
                .FirstOrDefault();

            if (attribute == null)
                return new ApiVersionDescriptor
                {
                    Versions = new IApiVersion[] { }
                };

            var versions = attribute.Versions
                .Select(x => _parser.TryParse(x, out var version) ? version : throw new FormatException())
                .OrderBy(x => x)
                .ToArray();

            return new ApiVersionDescriptor
            {
                DefaultVersion = attribute.DefaultVersion,
                ErrorMessage = $"Invalid api version, supported versions are {string.Join(", ", versions.Select(x => x.Text))}.",
                Optional = attribute.Optional,
                Versions = versions
            };
        }
    }
}