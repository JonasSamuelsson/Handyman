using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DependencyInjection.Diagnostics
{
    internal class ServiceProviderInsights : IServiceProviderInsights
    {
        private readonly IServiceCollection _services;

        public ServiceProviderInsights(IServiceCollection services)
        {
            _services = services;
        }

        public IEnumerable<ServiceDescription> GetServiceDescriptions()
        {
            return _services
                .Select(Map)
                .OrderBy(x => x.ServiceType.FullName)
                .ThenBy(x => x.ImplementationType.FullName);
        }

        private static ServiceDescription Map(ServiceDescriptor service)
        {
            return new ServiceDescription
            {
                ServiceType = service.ServiceType,
                ImplementationType = GetImplementationType(service),
                Lifetime = service.Lifetime,
                Kind = GetServiceKind(service)
            };
        }

        private static Type GetImplementationType(ServiceDescriptor service)
        {
            var type = service.ImplementationType
                       ?? service.ImplementationInstance?.GetType()
                       ?? service.ImplementationFactory?.Method.ReturnType;

            return type != typeof(object) ? type : null;
        }

        private static ServiceKind GetServiceKind(ServiceDescriptor service)
        {
            return service.ImplementationFactory != null
                ? ServiceKind.Factory
                : service.ImplementationInstance != null
                    ? ServiceKind.Instance
                    : ServiceKind.Type;
        }
    }
}