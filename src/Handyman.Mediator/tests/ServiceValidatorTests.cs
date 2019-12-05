using System;
using Handyman.Mediator.RequestPipelineCustomization;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class ServiceValidatorTests
    {
        [Fact]
        public void RequestWithSingleHandlerShouldPass()
        {
            var serviceTypes = new[]
            {
                typeof(IRequestHandler<DefaultPipelineRequest, Void>)
            };

            new ServiceValidator().Validate(serviceTypes);
        }

        [Fact]
        public void RequestWithMultipleHandlersAndDefaultPipelineShouldFail()
        {
            var serviceTypes = new[]
            {
                typeof(IRequestHandler<DefaultPipelineRequest, Void>),
                typeof(IRequestHandler<DefaultPipelineRequest, Void>)
            };

            Should.Throw<Exception>(() => new ServiceValidator().Validate(serviceTypes));
        }

        [Fact]
        public void RequestWithMultipleHandlersAndCustomPipelineShouldPass()
        {
            var serviceTypes = new[]
            {
                typeof(IRequestHandler<CustomizedPipelineRequest, Void>),
                typeof(IRequestHandler<CustomizedPipelineRequest, Void>)
            };

            new ServiceValidator().Validate(serviceTypes);
        }

        private class DefaultPipelineRequest : IRequest { }

        [PipelineBuilder]
        private class CustomizedPipelineRequest : IRequest { }

        private class PipelineBuilderAttribute : RequestPipelineBuilderAttribute
        {
            public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, IServiceProvider serviceProvider)
            {
                throw new NotImplementedException();
            }
        }
    }
}
