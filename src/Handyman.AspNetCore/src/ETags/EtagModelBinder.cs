using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.ETags
{
    internal class ETagModelBinder : IModelBinder
    {
        private readonly IETagValidator _validator;

        public ETagModelBinder(IETagValidator validator)
        {
            _validator = validator;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var headers = bindingContext.HttpContext.Request.Headers;

            if (headers.TryGetValue(bindingContext.BinderModelName, out var values))
            {
                var eTag = values.ToString();

                if (_validator.IsValidETag(eTag))
                {
                    bindingContext.Result = ModelBindingResult.Success(eTag);
                }
                else
                {
                    var key = bindingContext.ModelName;
                    var error = "Invalid ETag format.";
                    bindingContext.ModelState.AddModelError(key, error);
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }

            return Task.CompletedTask;
        }
    }
}