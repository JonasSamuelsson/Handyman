using System;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public interface IServiceRegistrationPolicy
    {
        void Register(Type type, IServiceCollection services);
    }
}