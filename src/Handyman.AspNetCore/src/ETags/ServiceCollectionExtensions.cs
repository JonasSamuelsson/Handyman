using Handyman.AspNetCore.ETags.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.AspNetCore.ETags
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddETags(this IServiceCollection services)
        {
            services.AddSingleton<IETagComparer, ETagComparer>();
            services.AddSingleton<IETagConverter, ETagConverter>();
            services.AddSingleton<IETagValidator, ETagValidator>();
            services.AddSingleton<IActionDescriptorProvider, ETagActionDescriptorProvider>();
            services.AddSingleton<ETagModelBinder>();
            services.AddControllers(options => options.ModelBinderProviders.Insert(0, new ETagModelBinderProvider()));

            return services;
        }
    }
}