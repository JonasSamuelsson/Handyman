using System.Collections.Generic;

namespace Handyman.DependencyInjection
{
    public interface IDiagnostics
    {
        IEnumerable<ServiceDescription> GetServiceDescriptions();
        string GetServiceDescriptionsString();
    }
}