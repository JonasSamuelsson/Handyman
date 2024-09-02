using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Handyman.AspNetCore.ApiVersioning.ModelBinding;

internal class ApiVersionModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        var metadata = context.Metadata;

        if (metadata.MetadataKind != ModelMetadataKind.Parameter)
            return null;

        if (metadata.ModelType != typeof(string))
            return null;

        if (!metadata.Name.Equals("apiVersion", StringComparison.OrdinalIgnoreCase))
            return null;

        return context.Services.GetRequiredService<ApiVersionModelBinder>();
    }
}