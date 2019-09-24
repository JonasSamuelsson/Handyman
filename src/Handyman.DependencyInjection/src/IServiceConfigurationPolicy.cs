using System;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public interface IServiceConfigurationPolicy
    {
        void Register(Type type, IServiceCollection services);
    }
}