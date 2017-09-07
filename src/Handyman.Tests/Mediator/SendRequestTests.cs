using Handyman.Mediator;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Tests.Mediator
{
    public class SendRequestTests
    {
        [Fact]
        public void ShouldSendRequestWithResponse()
        {
            var request = new RequestWithResponse();
            var serviceProvider = new ServiceProvider(typeof(IRequestHandler<RequestWithResponse, RequestWithResponse>), typeof(RequestWithResponseHandler));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);
            mediator.Send(request).Result.ShouldBe(request);
        }

        class RequestWithResponse : IRequest<RequestWithResponse> { }

        class RequestWithResponseHandler : IRequestHandler<RequestWithResponse, RequestWithResponse>
        {
            public Task<RequestWithResponse> Handle(RequestWithResponse request)
            {
                return Task.FromResult(request);
            }
        }

        [Fact]
        public void ShouldSendRequestWithoutResponse()
        {
            var request = new RequestWithoutResponse();
            var serviceProvider = new ServiceProvider(typeof(IRequestHandler<RequestWithoutResponse>), typeof(RequestWithoutResponseHandler));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);
            mediator.Send(request).Wait();
        }

        class RequestWithoutResponse : IRequest { }

        class RequestWithoutResponseHandler : IRequestHandler<RequestWithoutResponse>
        {
            public Task Handle(RequestWithoutResponse request)
            {
                return Task.CompletedTask;
            }
        }
    }
}