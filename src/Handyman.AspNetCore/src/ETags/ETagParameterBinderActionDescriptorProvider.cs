using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;

namespace Handyman.AspNetCore.ETags
{
    internal class ETagParameterBinderActionDescriptorProvider : IActionDescriptorProvider
    {
        internal static readonly Dictionary<string, string> HeaderLookup = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            {"ifMatch", HeaderNames.IfMatch},
            {"ifMatchETag", HeaderNames.IfMatch},
            {"ifNoneMatch", HeaderNames.IfNoneMatch},
            {"ifNoneMatchETag", HeaderNames.IfNoneMatch}
        };

        public int Order { get; } = 0;

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
        }

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
            foreach (var action in context.Results)
            {
                foreach (var parameter in action.Parameters)
                {
                    if (parameter.ParameterType != typeof(string))
                        continue;

                    if (!HeaderLookup.TryGetValue(parameter.Name, out var headerName))
                        continue;

                    parameter.BindingInfo.BindingSource = BindingSource.Header;
                    parameter.BindingInfo.BinderType = typeof(ETagModelBinder);
                    parameter.BindingInfo.BinderModelName = headerName;
                }
            }
        }
    }
}