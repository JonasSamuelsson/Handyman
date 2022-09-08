using Xunit;

namespace Handyman.DataContractValidator.Tests.Docs
{
    public class Anything
    {
        class MyParent
        {
            public string Name { get; set; }
            public MyChild Child { get; set; }
        }

        class MyChild
        {
            public string Text { get; set; }
        }

        [Fact]
        public void Validate()
        {
            var type = typeof(MyParent);

            var dataContract = typeof(Any);

            new DataContractValidator().Validate(type, dataContract);
        }
    }
}