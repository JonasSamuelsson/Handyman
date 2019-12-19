using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    internal interface IServiceConfigurationStrategy
    {
        void Execute(IServiceCollection services);
    }
}