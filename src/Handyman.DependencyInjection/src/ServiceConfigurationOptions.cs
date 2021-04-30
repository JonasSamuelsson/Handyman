using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public class ServiceConfigurationOptions
    {
        public ServiceConfigurationPolicy? ConfigurationPolicy { get; set; }
        public ServiceLifetime? Lifetime { get; set; }
    }
}