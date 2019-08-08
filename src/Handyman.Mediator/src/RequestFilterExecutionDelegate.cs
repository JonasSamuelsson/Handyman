using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public delegate Task<TResponse> RequestFilterExecutionDelegate<TResponse>();
}