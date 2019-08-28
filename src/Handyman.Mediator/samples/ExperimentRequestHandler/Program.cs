using Handyman.Mediator;
using Maestro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExperimentRequestHandler
{
    public class Program
    {
        private static readonly Random Random = new Random();

        public static async Task Main()
        {
            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Add, int>>().Type<AddHandler>();
                x.Add<IRequestHandler<Add, int>>().Type<NewAddHandler>();
            });

            var mediator = new Mediator(container.GetService);

            while (true)
            {
                Console.Write("press enter to execute");
                Console.ReadLine();

                var numbers = GetNumbers();
                Console.WriteLine($"Add: {string.Join(",", numbers)}");
                Console.WriteLine($"Sum: {await mediator.Send(new Add { Numbers = numbers })}");
                Console.WriteLine();
            }
        }

        private static List<int> GetNumbers()
        {
            var count = Random.Next(5) + 1;
            var numbers = new List<int>();

            for (var i = 0; i < count; i++)
            {
                numbers.Add(Random.Next(10));
            }

            return numbers;
        }
    }
}
