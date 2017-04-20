using Handyman.Mediator;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Tests.Mediator
{
    public class SendRequestTests
    {
        [Fact]
        public void ShouldProcessRequest()
        {
            var request = new TestRequest();
            var serviceProvider = new ServiceProvider(typeof(IRequestHandler<TestRequest, TestRequest>), typeof(TestRequestHandler));
            var mediator = new Handyman.Mediator.Mediator(serviceProvider.GetService, serviceProvider.GetServices);
            mediator.Send(request).Result.ShouldBe(request);
        }

        class TestRequest : IRequest<TestRequest> { }

        class TestRequestHandler : IRequestHandler<TestRequest, TestRequest>
        {
            public Task<TestRequest> Handle(TestRequest request)
            {
                return Task.FromResult(request);
            }
        }
    }
}