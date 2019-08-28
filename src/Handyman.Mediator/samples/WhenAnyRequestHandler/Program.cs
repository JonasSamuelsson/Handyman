using Maestro;
using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples.WhenAnyRequestHandler
{
    public class Program
    {
        private static Random Random = new Random();

        public static async Task Main()
        {
            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Type<BreadthFirst>();
                x.Add<IRequestHandler<Request, string>>().Type<DepthFirst>();
            });

            var mediator = new Mediator(container.GetService);

            var matrix = GetMatrix();

            while (true)
            {
                Console.Write("press enter to execute");
                Console.ReadLine();

                var text = GetText();
                Console.Write($"{text} : ");
                Console.WriteLine(await mediator.Send(new Request
                {
                    Matrix = matrix,
                    Text = text
                }));
                Console.WriteLine();
            }
        }

        private static string[,] GetMatrix()
        {
            var matrix = new string[10, 10];

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 10; y++)
                {
                    matrix[x, y] = $"{x}{y}";
                }
            }

            return matrix;
        }

        private static string GetText()
        {
            return $"{Random.Next(10)}{Random.Next(10)}";
        }
    }
}
