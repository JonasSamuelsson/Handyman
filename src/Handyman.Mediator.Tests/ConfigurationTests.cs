using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void EventPipelineShouldBeDisabledByDefault()
        {
            new Configuration().EventPipelineEnabled.ShouldBeFalse();
        }

        [Fact]
        public void RequestPipelineShouldBeDisabledByDefault()
        {
            new Configuration().RequestPipelineEnabled.ShouldBeFalse();
        }
    }
}