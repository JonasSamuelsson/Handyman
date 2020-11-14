using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class DynamicMediator : IDynamicMediator
    {
        private static readonly ConcurrentDictionary<Type, DynamicSender> DynamicSenders = new ConcurrentDictionary<Type, DynamicSender>();

        private readonly IMediator _mediator;

        public DynamicMediator(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Publish(object @event, CancellationToken cancellationToken)
        {
            return _mediator.Publish((IEvent)@event, cancellationToken);
        }

        public async Task<object?> Send(object request, CancellationToken cancellationToken)
        {
            var requestType = request.GetType();
            var dynamicSender = DynamicSenders.GetOrAdd(requestType, GetDynamicSender);
            return await dynamicSender.Send(request, _mediator, cancellationToken);
        }

        private static DynamicSender GetDynamicSender(Type requestType)
        {
            var responseType = GetResponseType(requestType);
            var dynamicSenderType = typeof(DynamicSender<>).MakeGenericType(responseType);
            return (DynamicSender)Activator.CreateInstance(dynamicSenderType);
        }

        private static Type GetResponseType(Type requestType)
        {
            var requestInterfaces = requestType.GetInterfaces()
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IRequest<>))
                .ToList();

            if (requestInterfaces.Count == 1)
            {
                return requestInterfaces[0].GetGenericArguments()[0];
            }

            var message = $"Unable to cast object of type '{requestType.FullName}' to type '{typeof(IRequest<>).FullName}'.";
            throw new InvalidCastException(message);
        }

        private abstract class DynamicSender
        {
            internal abstract Task<object?> Send(object request, ISender sender, CancellationToken cancellationToken);
        }

        private class DynamicSender<TResponse> : DynamicSender
        {
            internal override async Task<object?> Send(object request, ISender sender, CancellationToken cancellationToken)
            {
                return await sender.Send((IRequest<TResponse>)request, cancellationToken);
            }
        }
    }
}