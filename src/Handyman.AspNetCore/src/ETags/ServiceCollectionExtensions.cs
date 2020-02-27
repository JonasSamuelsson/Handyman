using Handyman.AspNetCore.ETags.ModelBinding;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Handyman.AspNetCore.ETags
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddETags(this IServiceCollection services)
        {
            if (services.Any(x => x.ServiceType == typeof(Sentinel)))
                throw new InvalidOperationException("ETags has already been added.");

            services.AddTransient<Sentinel>();
            services.AddSingleton<IETagComparer, ETagComparer>();
            services.AddSingleton<IETagConverter, ETagConverter>();
            services.AddSingleton<IETagValidator, ETagValidator>();
            services.AddSingleton<IActionDescriptorProvider, ETagActionDescriptorProvider>();
            services.AddSingleton<ETagModelBinder>();
            services.AddControllers(options => options.ModelBinderProviders.Insert(0, new ETagModelBinderProvider()));

            return services;
        }

        private class Sentinel { }
    }
}