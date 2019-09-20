using System;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public class ServiceDescription
    {
        public Type ServiceType { get; set; }
        public Type ImplementationType { get; set; }
        public ServiceLifetime Lifetime { get; set; }
        public ServiceKind Kind { get; set; }
    }
}