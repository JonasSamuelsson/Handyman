using Handyman.AspNetCore.ETags.Internals;
using Handyman.AspNetCore.ETags.Internals.AppModel;
using Handyman.AspNetCore.ETags.Internals.Middleware;
using Handyman.AspNetCore.ETags.Internals.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;

namespace Handyman.AspNetCore.ETags
{
    public static class ETagsServiceCollectionExtensions
    {
        public static IServiceCollection AddETags(this IServiceCollection services)
        {
            if (services.All(x => x.ServiceType != typeof(MarkerService)))
            {
                AddETagServices(services);
            }

            return services;
        }

        private static void AddETagServices(IServiceCollection services)
        {
            services.AddTransient<MarkerService>();

            services.TryAddSingleton<IETagComparer, ETagComparer>();
            services.TryAddSingleton<IETagConverter, ETagConverter>();
            services.TryAddSingleton<IETagUtilities, ETagUtilities>();
            services.TryAddSingleton<IETagValidator, ETagValidator>();

            services.AddSingleton<ETagModelBinder>();
            services.AddSingleton<ETagValidatorMiddleware>();
            services.AddSingleton<PreconditionFailedExceptionHandlerMiddleware>();
            services.AddSingleton<ProblemDetailsResponseWriter>();

            services.AddControllers(options =>
            {
                options.Conventions.Add(new ETagParameterConvention());
                options.ModelBinderProviders.Insert(0, new ETagModelBinderProvider());
            });
        }

        private class MarkerService { }
    }
}