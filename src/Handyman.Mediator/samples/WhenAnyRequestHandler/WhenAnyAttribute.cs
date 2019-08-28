using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Samples.WhenAnyRequestHandler
{
    public class WhenAnyAttribute : RequestHandlerProviderAttribute
    {
        public override IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider)
        {
            if (typeof(TRequest).GetCustomAttributes<WhenAnyAttribute>().Any())
            {
                var handlers = GetService<IEnumerable<IRequestHandler<TRequest, TResponse>>>(serviceProvider);
                return new WhenAnyHandler<TRequest, TResponse>(handlers);
            }

            return GetService<IRequestHandler<TRequest, TResponse>>(serviceProvider);
        }

        private static T GetService<T>(ServiceProvider serviceProvider)
        {
            return (T)serviceProvider.Invoke(typeof(T));
        }
    }
}