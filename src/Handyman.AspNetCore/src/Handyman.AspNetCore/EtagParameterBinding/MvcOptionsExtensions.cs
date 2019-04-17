using Microsoft.AspNetCore.Mvc;

namespace Handyman.AspNetCore.EtagParameterBinding
{
    public static class MvcOptionsExtensions
    {
        public static void AddEtagParameterBinding(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new EtagModelBinderProvider());
        }
    }
}