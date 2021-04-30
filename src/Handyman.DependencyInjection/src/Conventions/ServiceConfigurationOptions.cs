using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection.Conventions
{
    public class ServiceConfigurationOptions
    {
        public ServiceConfigurationPolicy? ServiceConfigurationPolicy { get; set; }
        public ServiceLifetime? ServiceLifetime { get; set; }
    }
}