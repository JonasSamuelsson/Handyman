using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestFilterToggle
{
    public interface IRequestFilterToggle
   {
      Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata,
          RequestContext<TRequest> requestContext)
          where TRequest : IRequest<TResponse>;
   }
}