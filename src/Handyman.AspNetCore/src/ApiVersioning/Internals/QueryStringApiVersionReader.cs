using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning.Internals
{
    internal class QueryStringApiVersionReader : IApiVersionReader
    {
        public bool TryRead(HttpRequest httpRequest, out StringValues values)
        {
            return httpRequest.Query.TryGetValue("api-version", out values);
        }

        public ApiVersionSource GetApiVersionSourceOrNull()
        {
            return new ApiVersionSource
            {
                BindingSource = BindingSource.Query,
                Name = "api-version"
            };
        }
    }
}