using Handyman.Mediator.Internals;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void ShouldReturnDefaultEventFilterProviderByDefault()
        {
            new Configuration().GetEventFilterProvider().ShouldBe(DefaultEventFilterProvider.Instance);
        }

        [Fact]
        public void ShouldReturnDefaultEventHandlerProviderByDefault()
        {
            new Configuration().GetEventHandlerProvider().ShouldBe(DefaultEventHandlerProvider.Instance);
        }
    }
}