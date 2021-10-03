using Xunit;

namespace Handyman.DataContractValidator.Tests.Validation
{
    public class RegisteredDataContractsTests
    {
        [Fact]
        public void ShouldValidateUsingDefinition()
        {
            var store = new DataContractStore();

            store.Add("x", new
            {
                Text = typeof(string)
            });

            new DataContractValidator().Validate(typeof(Root), new
            {
                Child = store.Get("x")
            });
        }

        [Fact]
        public void ShouldBeAbleToRegisterDataContractsInAnyOrder()
        {
            var store = new DataContractStore();

            store.Add("root", new
            {
                Child = store.Get("child")
            });

            store.Add("child", new
            {
                Text = typeof(string)
            });

            new DataContractValidator().Validate(typeof(Root), store.Get("root"));
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
