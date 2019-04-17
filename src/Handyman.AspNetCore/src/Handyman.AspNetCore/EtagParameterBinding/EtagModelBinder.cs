using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;

namespace Handyman.AspNetCore.EtagParameterBinding
{
    internal class EtagModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (TryGetEtag(bindingContext.HttpContext.Request.Headers, out var etag))
                bindingContext.Result = ModelBindingResult.Success(etag);

            return Task.CompletedTask;
        }

        private static bool TryGetEtag(IHeaderDictionary headers, out string etag)
        {
            const string inm = HeaderNames.IfNoneMatch;
            const string im = HeaderNames.IfMatch;

            if (headers.TryGetValue(inm, out var values) || headers.TryGetValue(im, out values))
            {
                etag = values.ToString();
                return true;
            }

            etag = null;
            return false;
        }
    }
}