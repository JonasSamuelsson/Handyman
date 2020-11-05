using Handyman.DependencyInjection;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public static class Program
    {
        public static async Task Main()
        {
            while (true)
            {
                var samples = GetSamples()
                    .OrderBy(x => x.Order)
                    .Select((sample, i) =>
                    {
                        var index = (i + 1).ToString();
                        Console.WriteLine($"{index}: {sample.Name}");
                        return new { sample, index };
                    })
                    .ToDictionary(x => x.index, x => x.sample);

                Console.WriteLine();
                Console.Write("Select sample to run ([enter] to exit): ");

                var input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                    return;

                if (!samples.TryGetValue(input, out var sample))
                {
                    Console.WriteLine("Invalid selection");
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine();

                var sampleTypes = sample.GetType().GetNestedTypes();
                var serviceProvider = new ServiceCollection()
                    .AddMediator(options => options.ScanTypes(sampleTypes))
                    .Scan(scanner =>
                    {
                        scanner.Types(sampleTypes);
                        scanner.ConfigureConcreteClassesOf<ISample>();
                        scanner.ConfigureClassesWithServiceAttribute();
                    })
                    .BuildServiceProvider();

                await sample.Run(serviceProvider.GetRequiredService<IMediator>());

                Console.WriteLine();
                Console.WriteLine("===================================================");
                Console.WriteLine();
            }
        }

        private static IEnumerable<ISample> GetSamples()
        {
            return typeof(Program).Assembly
                .GetTypes()
                .Where(x => x.GetInterfaces().Any(x => x == typeof(ISample)))
                .Select(x => (ISample)Activator.CreateInstance(x));
        }
    }

    public interface ISample
    {
        int Order { get; }
        string Name { get; }

        Task Run(IMediator mediator);
    }
}
