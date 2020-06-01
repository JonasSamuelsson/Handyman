using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ETags.ModelBinding
{
    internal class ETagModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var headers = bindingContext.HttpContext.Request.Headers;

            if (headers.TryGetValue(bindingContext.BinderModelName, out var values))
            {
                bindingContext.Result = ModelBindingResult.Success(values.ToString());
            }

            return Task.CompletedTask;
        }
    }
}