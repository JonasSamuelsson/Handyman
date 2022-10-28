using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public interface IServiceConfigurationStrategy
    {
        void Execute(IServiceCollection services);
    }
}