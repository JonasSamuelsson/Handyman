using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;

namespace Handyman.AspNetCore.ApiVersioning
{
    // ReSharper disable once RedundantAttributeUsageProperty
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal sealed class ApiVersionAttribute : Attribute, IFilterFactory
    {
        private readonly StringValues _validVersions;

        public ApiVersionAttribute(string version)
            : this(new[] { version })
        {
        }

        public ApiVersionAttribute(string[] versions)
        {
            _validVersions = versions;
        }

        public bool Optional { get; set; }
        public Type Validator { get; set; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var apiVersionReader = serviceProvider.GetRequiredService<IApiVersionReader>();
            var apiVersionValidator = (IApiVersionValidator)serviceProvider.GetRequiredService(Validator ?? typeof(IApiVersionValidator));

            return new ApiVersionValidationFilter(_validVersions, Optional, apiVersionReader, apiVersionValidator);
        }

        bool IFilterFactory.IsReusable { get; } = true;
    }
}
