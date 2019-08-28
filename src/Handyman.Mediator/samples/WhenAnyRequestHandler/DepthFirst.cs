using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples.WhenAnyRequestHandler
{
    public class DepthFirst : IRequestHandler<Request, string>
    {
        public async Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            var width = request.Matrix.GetLength(0);
            var height = request.Matrix.GetLength(1);

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    await Task.Delay(5, cancellationToken);

                    if (request.Matrix[x, y] != request.Text)
                        continue;

                    return $"depth first : {x},{y}";
                }
            }

            return "depth first : not found";
        }
    }
}