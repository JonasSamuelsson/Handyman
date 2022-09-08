using Xunit;

namespace Handyman.DataContractValidator.Tests.Docs
{
    public class ValueTypes
    {
        [Fact]
        public void Validate()
        {
            var type = typeof(int);

            var dataContract = typeof(int);

            new DataContractValidator().Validate(type, dataContract);
        }
    }
}