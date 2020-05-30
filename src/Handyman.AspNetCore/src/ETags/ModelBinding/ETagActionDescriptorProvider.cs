using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;

namespace Handyman.AspNetCore.ETags.ModelBinding
{
    internal class ETagActionDescriptorProvider : IActionDescriptorProvider
    {
        private static readonly Dictionary<string, string> HeaderNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            {"ifMatch", Microsoft.Net.Http.Headers.HeaderNames.IfMatch},
            {"ifMatchETag", Microsoft.Net.Http.Headers.HeaderNames.IfMatch},
            {"ifNoneMatch", Microsoft.Net.Http.Headers.HeaderNames.IfNoneMatch},
            {"ifNoneMatchETag", Microsoft.Net.Http.Headers.HeaderNames.IfNoneMatch}
        };

        public int Order { get; } = 0;

        public void OnProvidersExecuted(ActionDescriptorProviderContext context)
        {
        }

        public void OnProvidersExecuting(ActionDescriptorProviderContext context)
        {
            foreach (var action in context.Results)
            {
                foreach (var parameter in action.Parameters)
                {
                    if (parameter.ParameterType != typeof(string))
                        continue;

                    if (!HeaderNames.TryGetValue(parameter.Name, out var headerName))
                        continue;

                    parameter.BindingInfo = new BindingInfo
                    {
                        BinderModelName = headerName,
                        BindingSource = BindingSource.Header,
                        BinderType = typeof(ETagModelBinder)
                    };
                }
            }
        }
    }
}