using Handyman.AspNetCore.ETags.Internals;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Linq;
using Handyman.AspNetCore.ETags.Internals.AppModel;
using Handyman.AspNetCore.ETags.Internals.Middleware;
using Handyman.AspNetCore.ETags.Internals.ModelBinding;

namespace Handyman.AspNetCore.ETags
{
    public static class ETagsServiceCollectionExtensions
    {
        public static IServiceCollection AddETags(this IServiceCollection services)
        {
            services.TryAddSingleton<IETagComparer, ETagComparer>();
            services.TryAddSingleton<IETagConverter, ETagConverter>();
            services.TryAddSingleton<IETagValidator, ETagValidator>();
            services.TryAddSingleton<ETagModelBinder>();
            services.TryAddSingleton<ETagValidatorMiddleware>();
            services.TryAddSingleton<PreconditionFailedExceptionHandlerMiddleware>();
            services.TryAddSingleton<ProblemDetailsResponseWriter>();

            services.AddControllers(options =>
            {
                if (options.Conventions.OfType<NoOpConvention>().Any() == false)
                {
                    options.Conventions.Add(new NoOpConvention());
                    options.Conventions.Add(new ETagParameterConvention());
                }

                if (options.ModelBinderProviders.OfType<ETagModelBinderProvider>().Any() == false)
                {
                    options.ModelBinderProviders.Insert(0, new ETagModelBinderProvider());
                }
            });

            return services;
        }

        public class NoOpConvention : IApplicationModelConvention
        {
            public void Apply(ApplicationModel application) { }
        }
    }
}