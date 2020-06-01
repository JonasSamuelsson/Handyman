using Handyman.AspNetCore.ETags.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Handyman.AspNetCore.ETags
{
    public static class ETagsApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseETags(this IApplicationBuilder app)
        {
            return app
                .UseMiddleware<ETagValidatorMiddleware>()
                .UseMiddleware<PreconditionFailedExceptionHandlerMiddleware>();
        }
    }
}