using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ApiVersioning.ModelBinding;

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