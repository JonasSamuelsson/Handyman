using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace Handyman.AspNetCore.ETags;

public static class HttpResponseExtensions
{
    public static void SetETagHeader(this HttpResponse response, string eTag)
    {
        eTag = ETagUtility.ToETag(eTag);

        if (!ETagUtility.IsValidETag(eTag))
        {
            ETagUtility.ThrowInvalidETagException();
        }

        response.Headers[HeaderNames.ETag] = eTag;
    }
}