using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples.WhenAnyRequestHandler
{
    public class BreadthFirst : IRequestHandler<Request, string>
    {
        public async Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            var width = request.Matrix.GetLength(0);
            var height = request.Matrix.GetLength(1);

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    await Task.Delay(5, cancellationToken);

                    if (request.Matrix[x, y] != request.Text)
                        continue;

                    return $"breadth first : {x},{y}";
                }
            }

            return "breadth first : not found";
        }
    }
}