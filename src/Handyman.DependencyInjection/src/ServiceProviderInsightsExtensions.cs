using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handyman.DependencyInjection
{
    public static class ServiceProviderInsightsExtensions
    {
        public static string ListServiceDescriptions(this IServiceProviderInsights serviceProviderInsights)
        {
            var lines = new List<string[]> { new[] { "Kind", "Lifetime", "Service type", "Implementation type" } };

            foreach (var description in serviceProviderInsights.GetServiceDescriptions())
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
    }
}