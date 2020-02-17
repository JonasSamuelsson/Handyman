using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.AspNetCore.ETags
{
    internal class ETagModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            return context.Services.GetRequiredService<ETagModelBinder>();
        }
    }
}