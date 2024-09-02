using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.AspNetCore.ETags.Internals.ModelBinding;

internal class ETagModelBinderProvider : IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        return context.BindingInfo.BinderType == typeof(ETagModelBinder)
            ? context.Services.GetRequiredService<ETagModelBinder>()
            : null;
    }
}