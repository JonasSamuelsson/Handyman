using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Handyman.AspNetCore.ApiVersioning.Filters
{
    internal class ApiVersionParameterBinderFilter : ActionFilterAttribute
    {
        private readonly string _parameterName;

        public ApiVersionParameterBinderFilter(string parameterName)
        {
            _parameterName = parameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            PopulateActionParameter(context);
            base.OnActionExecuting(context);
        }

        private void PopulateActionParameter(ActionExecutingContext context)
        {
            var feature = context.HttpContext.Features.Get<ApiVersionFeature>();

            if (feature == null)
                throw new InvalidOperationException();

            var version = feature.Version;

            if (string.IsNullOrWhiteSpace(version))
                return;

            context.ActionArguments[_parameterName] = version;
        }
    }
}