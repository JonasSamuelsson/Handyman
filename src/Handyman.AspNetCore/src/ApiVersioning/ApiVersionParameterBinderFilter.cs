using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal class ApiVersionParameterBinderFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            PopulateActionParameter(context);
            base.OnActionExecuting(context);
        }

        private static void PopulateActionParameter(ActionExecutingContext context)
        {
            var feature = context.HttpContext.Features.Get<ApiVersionFeature>();

            if (feature == null)
                throw new InvalidOperationException();

            var version = feature.Version;

            if (string.IsNullOrWhiteSpace(version))
                return;

            var parameters = context.ActionDescriptor.Parameters;

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < parameters.Count; i++)
            {
                var parameter = parameters[i];

                if (parameter.ParameterType != typeof(string))
                    continue;

                var name = parameter.Name;

                if (name.Equals("apiversion", StringComparison.OrdinalIgnoreCase) == false)
                    continue;

                context.ActionArguments[name] = version;
                return;
            }
        }
    }
}