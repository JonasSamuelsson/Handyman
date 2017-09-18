using Handyman.Mediator;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Tests.Mediator
{
    public class SendRequestTests
    {
        [Fact]
        public void ShouldSendRequestWithoutResponse()
        {
            var request = new RequestWithoutResponse();
            var serviceProvider = new ServiceProvider(typeof(IRequestHandler<RequestWithoutResponse>), typeof(RequestWithoutResponseHandler));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);
            mediator.Send(request);
        }

        class RequestWithoutResponse : IRequest { }

        class RequestWithoutResponseHandler : IRequestHandler<RequestWithoutResponse>
        {
            public void Handle(RequestWithoutResponse request) { }
        }

        [Fact]
        public void ShouldSendRequestWithResponse()
        {
            var request = new RequestWithResponse();
            var serviceProvider = new ServiceProvider(typeof(IRequestHandler<RequestWithResponse, RequestWithResponse>), typeof(RequestWithResponseHandler));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);
            mediator.Send(request).ShouldBe(request);
        }

        class RequestWithResponse : IRequest<RequestWithResponse> { }

        class RequestWithResponseHandler : IRequestHandler<RequestWithResponse, RequestWithResponse>
        {
            public RequestWithResponse Handle(RequestWithResponse request)
            {
                return request;
            }
        }

        [Fact]
        public void ShouldSendAsyncRequestWithoutResponse()
        {
            var request = new AsyncRequestWithoutResponse();
            var serviceProvider = new ServiceProvider(typeof(IRequestHandler<AsyncRequestWithoutResponse, Task>), typeof(AsyncRequestWithoutResponseHandler));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);
            mediator.Send(request).Wait();
        }

        class AsyncRequestWithoutResponse : IAsyncRequest { }

        class AsyncRequestWithoutResponseHandler : IAsyncRequestHandler<AsyncRequestWithoutResponse>
        {
            public Task Handle(AsyncRequestWithoutResponse request)
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public void ShouldSendAsyncRequestWithResponse()
        {
            var request = new AsyncRequestWithResponse();
            var serviceProvider = new ServiceProvider(typeof(IRequestHandler<AsyncRequestWithResponse, Task<AsyncRequestWithResponse>>), typeof(AsyncRequestWithResponseHandler));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);
            mediator.Send(request).Result.ShouldBe(request);
        }

        class AsyncRequestWithResponse : IAsyncRequest<AsyncRequestWithResponse> { }

        class AsyncRequestWithResponseHandler : IAsyncRequestHandler<AsyncRequestWithResponse, AsyncRequestWithResponse>
        {
            public Task<AsyncRequestWithResponse> Handle(AsyncRequestWithResponse request)
            {
                return Task.FromResult(request);
            }
        }
    }
}