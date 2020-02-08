using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Handyman.AspNetCore.EtagParameterBinding
{
    internal class EtagModelBinder : IModelBinder
    {
        private readonly string _parameterName;
        private readonly string _headerName;
        private readonly IETagValidator _validator;

        public EtagModelBinder(string parameterName, string headerName, IETagValidator validator)
        {
            _parameterName = parameterName;
            _headerName = headerName;
            _validator = validator;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var headers = bindingContext.HttpContext.Request.Headers;

            if (headers.TryGetValue(_headerName, out var eTag))
            {
                if (_validator.IsValidETag(eTag))
                {
                    bindingContext.Result = ModelBindingResult.Success(eTag);
                }
                else
                {
                    bindingContext.ModelState.AddModelError(_parameterName, $"Header {_headerName} contains invalid ETag.");
                    bindingContext.Result = ModelBindingResult.Failed();
                }
            }

            return Task.CompletedTask;
        }
    }
}