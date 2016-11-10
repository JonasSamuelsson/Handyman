using System.Threading.Tasks;

namespace Handyman.Dispatch
{
   public interface IRequestHandler<TRequest, TResponse>
      where TRequest : IRequest<TResponse>
   {
      Task<TResponse> Handle(TRequest request);
   }
}