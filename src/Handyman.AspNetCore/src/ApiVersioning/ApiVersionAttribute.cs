using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System;

namespace Handyman.AspNetCore.ApiVersioning
{
    // ReSharper disable once RedundantAttributeUsageProperty
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ApiVersionAttribute : Attribute, IFilterFactory
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

        /// <summary>
        /// Specify the default version to use if api version is optional and not provided in the request.
        /// </summary>
        public string DefaultVersion { get; set; }

        /// <summary>
        /// Specify if an api version is optional or not, default is false.
        /// </summary>
        public bool Optional { get; set; }

        /// <summary>
        /// Custom validator type used for validation.
        /// </summary>
        public Type Validator { get; set; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var apiVersionReader = serviceProvider.GetRequiredService<IApiVersionReader>();
            var apiVersionValidator = (IApiVersionValidator)serviceProvider.GetRequiredService(Validator ?? typeof(IApiVersionValidator));

            return new ApiVersionValidationFilter(_validVersions, Optional, DefaultVersion, apiVersionReader, apiVersionValidator);
        }

        bool IFilterFactory.IsReusable { get; } = true;
    }
}
