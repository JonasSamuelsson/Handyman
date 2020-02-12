using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Handyman.AspNetCore.ApiVersioning.Filters
{
    internal class ApiVersionValidatorFilterFactory : IFilterFactory
    {
        public bool IsReusable { get; } = true;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<ApiVersionValidatorFilter>();
        }
    }
}