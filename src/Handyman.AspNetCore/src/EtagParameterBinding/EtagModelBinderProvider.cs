using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Handyman.AspNetCore.EtagParameterBinding
{
    internal class EtagModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType != typeof(string))
                return null;

            if (context.Metadata.ParameterName.Equals("etag", StringComparison.OrdinalIgnoreCase) == false)
                return null;

            return new EtagModelBinder();
        }
    }
}