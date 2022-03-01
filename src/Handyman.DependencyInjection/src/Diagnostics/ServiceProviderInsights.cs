using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handyman.DependencyInjection.Diagnostics
{
    internal class ServiceProviderInsights : IServiceProviderInsights
    {
        private readonly IServiceCollection _services;

        public ServiceProviderInsights(IServiceCollection services)
        {
            _services = services;
        }

        public IEnumerable<ServiceDescription> GetServiceDescriptions()
        {
            return _services
                .OrderBy(x => x.ServiceType.FullName)
                .ThenBy(x => GetImplementationType(x)?.FullName ?? string.Empty)
                .Select(Map)
                .ToList();
        }

        public string ListServiceDescriptions()
        {
            var lines = new List<string[]> { new[] { "Kind", "Lifetime", "Service type", "Implementation type" } };

            foreach (var description in GetServiceDescriptions())
            {
                var kind = description.Kind.ToString();
                var lifetime = description.Lifetime.ToString();
                var serviceType = description.ServiceType.PrettyPrint();
                var implementationType = description.ImplementationType?.PrettyPrint() ?? "unknown";

                lines.Add(new[] { kind, lifetime, serviceType, implementationType });
            }

            var columnIndexes = Enumerable.Range(0, 4).ToList();
            var columnWidths = columnIndexes.Select(i => lines.Max(x => x[i].Length) + 1).ToList();

            var builder = new StringBuilder();

            AddLine(lines[0]);
            AddLine(columnWidths.Select(i => new string('=', i)).ToArray());

            foreach (var line in lines.Skip(1))
            {
                AddLine(line);
            }

            return builder.ToString();

            void AddLine(IReadOnlyList<string> strings)
            {
                foreach (var i in columnIndexes)
                {
                    builder.Append($"{strings[i].PadRight(columnWidths[i])}");
                }

                builder.AppendLine();
            }
        }

        private static ServiceDescription Map(ServiceDescriptor service)
        {
            return new ServiceDescription
            {
                ServiceType = service.ServiceType,
                ImplementationType = GetImplementationType(service),
                Lifetime = service.Lifetime,
                Kind = GetServiceKind(service)
            };
        }

        private static Type GetImplementationType(ServiceDescriptor service)
        {
            var type = service.ImplementationType
                       ?? service.ImplementationInstance?.GetType()
                       ?? service.ImplementationFactory?.Method.ReturnType;

            return type != typeof(object) ? type : null;
        }

        private static ServiceKind GetServiceKind(ServiceDescriptor service)
        {
            return service.ImplementationFactory != null
                ? ServiceKind.Factory
                : service.ImplementationInstance != null
                    ? ServiceKind.Instance
                    : ServiceKind.Type;
        }
    }
}