using Handyman.Mediator;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Tests.Mediator
{
    public class ProcessRequestTests
    {
        [Fact]
        public void ShouldProcessRequest()
        {
            var request = new TestRequest();
            var handlerProvider = new TestHandlerProvider(typeof(IRequestHandler<TestRequest, TestRequest>), typeof(TestRequestHandler));
            var mediator = new Handyman.Mediator.Mediator(handlerProvider);
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