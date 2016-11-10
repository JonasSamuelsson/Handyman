using System.Threading.Tasks;
using Handyman.Dispatch;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Dispatch
{
   public class ProcessRequestTests
   {
      [Fact]
      public void ShouldProcessRequest()
      {
         var request = new TestRequest();
         var handlerProvider = new TestHandlerProvider(typeof(IRequestHandler<TestRequest, TestRequest>), typeof(TestRequestHandler));
         var dispatcher = new Dispatcher(handlerProvider);
         dispatcher.Process(request).Result.ShouldBe(request);
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