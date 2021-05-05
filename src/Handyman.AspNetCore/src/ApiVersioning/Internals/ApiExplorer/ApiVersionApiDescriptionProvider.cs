using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning.Internals.ApiExplorer
{
    internal class ApiVersionApiDescriptionProvider : IApiDescriptionProvider
    {
        private readonly IEnumerable<IApiVersionReader> _apiVersionReaders;
        private readonly ApiDescriptionGroupings _apiDescriptionGroupings;

        public ApiVersionApiDescriptionProvider(IEnumerable<IApiVersionReader> apiVersionReaders, ApiDescriptionGroupings apiDescriptionGroupings)
        {
            _apiVersionReaders = apiVersionReaders;
            _apiDescriptionGroupings = apiDescriptionGroupings;
        }

        public int Order => int.MinValue;

        public void OnProvidersExecuting(ApiDescriptionProviderContext context)
        {
            var apiVersionSources = _apiVersionReaders
                .Select(x => x.GetApiVersionSourceOrNull())
                .Where(x => x != null)
                .ToList();

            if (apiVersionSources.Count == 0)
                return;

            foreach (var apiDescription in context.Results)
            {
                var apiVersionDescriptor = apiDescription.ActionDescriptor.EndpointMetadata
                    .OfType<ApiVersionMetadata>()
                    .SingleOrDefault();

                if (apiVersionDescriptor == null)
                    continue;

                foreach (var parameter in apiDescription.ParameterDescriptions.ToList())
                {
                    if (parameter.Type != typeof(string))
                        continue;

                    if (!parameter.Name.Equals("apiVersion", StringComparison.OrdinalIgnoreCase))
                        continue;

                    apiDescription.ParameterDescriptions.Remove(parameter);
                }

                foreach (var apiVersionSource in apiVersionSources)
                {
                    var apiParameterDescription = new ApiParameterDescription
                    {
                        DefaultValue = apiVersionDescriptor.DefaultVersion,
                        IsRequired = !apiVersionDescriptor.IsOptional,
                        Name = apiVersionSource.Name,
                        Source = apiVersionSource.BindingSource,
                        Type = typeof(string)
                    };

                    apiDescription.ParameterDescriptions.Add(apiParameterDescription);
                }
            }
        }

        public void OnProvidersExecuted(ApiDescriptionProviderContext context)
        {
            foreach (var g in context.Results.GroupBy(x => $"{x.HttpMethod} {x.RelativePath}").Where(x => x.Count() != 1))
            {
                var index = -1;

                foreach (var apiDescription in g.OrderByDescending(x => x, Comparer.Instance))
                {
                    apiDescription.GroupName = index++ == -1 ? "vLatest" : $"vLatest-{index}";

                    if (!_apiDescriptionGroupings.GroupNames.Contains(apiDescription.GroupName))
                    {
                        _apiDescriptionGroupings.GroupNames.Add(apiDescription.GroupName);
                    }
                }

                //foreach (var apiDescription in g.OrderByDescending(x => x, Comparer.Instance).Skip(1))
                //{
                //    context.Results.Remove(apiDescription);
                //}
            }
        }
    }

    public class ApiDescriptionGroupings
    {
        public List<string> GroupNames { get; set; } = new List<string>();
    }
}