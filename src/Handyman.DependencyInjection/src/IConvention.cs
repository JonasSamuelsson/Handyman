using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public interface IConvention
    {
        void Execute(IReadOnlyCollection<Type> types, IServiceCollection services);
    }
}