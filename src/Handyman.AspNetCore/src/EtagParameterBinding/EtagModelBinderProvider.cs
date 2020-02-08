using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Handyman.AspNetCore.EtagParameterBinding
{
    internal class EtagModelBinderProvider : IModelBinderProvider
    {
        private readonly Dictionary<string, string> _headerNameDictionary = new Dictionary<string, string>
        {
            {"ifmatch", HeaderNames.IfMatch },
            {"ifmatchetag", HeaderNames.IfMatch },
            {"ifnonematch", HeaderNames.IfNoneMatch },
            {"ifnonematchetag", HeaderNames.IfNoneMatch },
        };
        private readonly ConcurrentDictionary<string, IModelBinder> _modelBinderDictionary = new ConcurrentDictionary<string, IModelBinder>();

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(string))
                return null;

            var parameterName = context.Metadata.ParameterName.ToLowerInvariant();

            if (_headerNameDictionary.TryGetValue(parameterName, out var headerName))
                return _modelBinderDictionary.GetOrAdd(parameterName, x => CreateModelBinder(x, headerName, context.Services));

            return null;
        }

        private static IModelBinder CreateModelBinder(string parameterName, string headerName, IServiceProvider services)
        {
            var validator = services.GetRequiredService<IETagValidator>();
            return new EtagModelBinder(parameterName, headerName, validator);
        }
    }
}