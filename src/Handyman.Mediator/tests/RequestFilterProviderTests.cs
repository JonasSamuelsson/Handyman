using Shouldly;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class RequestFilterProviderTests
    {
        [Fact]
        public void ShouldProviderAnOrderedListOfFilters()
        {
            var serviceProvider = new ServiceProvider(type => new IRequestFilter<Request, object>[]
            {
                new FilterC(),
                new FilterB(),
                new FilterA()
            });

            var filters = new RequestFilterProvider()
                .GetFilters<Request, object>(serviceProvider)
                .ToList();

            filters[0].ShouldBeOfType<FilterA>();
            filters[1].ShouldBeOfType<FilterB>();
            filters[2].ShouldBeOfType<FilterC>();
        }

        private class Request : IRequest<object> { }

        private class FilterA : IOrderedPipelineHandler, IRequestFilter<Request, object>
        {
            public int Order => -1;

            public Task<object> Execute(RequestFilterContext<Request> context, Func<Task<object>> next)
            {
                throw new NotImplementedException();
            }
        }

        private class FilterB : IRequestFilter<Request, object>
        {
            public Task<object> Execute(RequestFilterContext<Request> context, Func<Task<object>> next)
            {
                throw new NotImplementedException();
            }
        }

        private class FilterC : IOrderedPipelineHandler, IRequestFilter<Request, object>
        {
            public int Order => 1;

            public Task<object> Execute(RequestFilterContext<Request> context, Func<Task<object>> next)
            {
                throw new NotImplementedException();
            }
        }
    }
}