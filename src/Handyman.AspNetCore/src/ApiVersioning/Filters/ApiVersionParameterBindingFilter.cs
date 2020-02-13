using Microsoft.AspNetCore.Mvc.Filters;

namespace Handyman.AspNetCore.ApiVersioning.Filters
{
    internal class ApiVersionParameterBindingFilter : ActionFilterAttribute
    {
        private readonly string _parameterName;

        public ApiVersionParameterBindingFilter(string parameterName)
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
                return;

            context.ActionArguments[_parameterName] = feature.MatchedVersion;
        }
    }
}