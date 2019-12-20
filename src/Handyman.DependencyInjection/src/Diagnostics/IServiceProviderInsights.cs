using System.Collections.Generic;

namespace Handyman.DependencyInjection.Diagnostics
{
    public interface IServiceProviderInsights
    {
        IEnumerable<ServiceDescription> GetServiceDescriptions();
    }
}