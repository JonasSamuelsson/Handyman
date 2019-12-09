using System.Collections.Generic;

namespace Handyman.DependencyInjection
{
    public interface IServiceProviderInsights
    {
        IEnumerable<ServiceDescription> GetServiceDescriptions();
    }
}