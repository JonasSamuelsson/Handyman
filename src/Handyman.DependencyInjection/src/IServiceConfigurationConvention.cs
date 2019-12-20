using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Handyman.DependencyInjection
{
    public interface IServiceConfigurationConvention
    {
        void Execute(IReadOnlyCollection<Type> types, IServiceCollection services);
    }
}