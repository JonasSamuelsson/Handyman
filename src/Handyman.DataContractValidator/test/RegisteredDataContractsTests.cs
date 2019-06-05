using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class RegisteredDataContractsTests
    {
        [Fact]
        public void ShouldValidateUsingDefinition()
        {
            var validator = new DataContractValidator();

            validator.AddDataContract("x", new
            {
                Text = typeof(string)
            });

            validator.Validate(typeof(Root), new
            {
                Child = validator.GetDataContract("x")
            });
        }

        [Fact]
        public void ShouldBeAbleToRegisterDataContractsInAnyOrder()
        {
            var validator = new DataContractValidator();

            validator.AddDataContract("root", new
            {
                Child = validator.GetDataContract("child")
            });

            validator.AddDataContract("child", new
            {
                Text = typeof(string)
            });

            validator.Validate(typeof(Root), validator.GetDataContract("root"));
        }

        private class Root
        {
            public Child Child { get; set; }
        }

        private class Child
        {
            public string Text { get; set; }
        }
    }
}
