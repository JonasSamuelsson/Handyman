using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Handyman.AspNetCore.ApiVersioning.Internals.ModelBinding
{
    internal class ApiVersionModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var feature = bindingContext.HttpContext.Features.Get<ApiVersionFeature>();

            if (feature != null)
            {
                bindingContext.Result = ModelBindingResult.Success(feature.MatchedVersion);
            }

            return Task.CompletedTask;
        }
    }
}