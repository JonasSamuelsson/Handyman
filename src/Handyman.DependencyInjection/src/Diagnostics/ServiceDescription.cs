using Microsoft.Extensions.DependencyInjection;
using System;

namespace Handyman.DependencyInjection.Diagnostics
{
    public class ServiceDescription
    {
        public Type ServiceType { get; set; }
        public Type ImplementationType { get; set; }
        public ServiceLifetime Lifetime { get; set; }
        public ServiceKind Kind { get; set; }
    }
}