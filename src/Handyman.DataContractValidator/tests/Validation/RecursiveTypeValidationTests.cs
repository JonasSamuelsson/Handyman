using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Validation
{
    public class RecursiveTypeValidationTests
    {
        [Fact]
        public void ShouldValidateRecursiveTypes()
        {
            var dataContractStore = new DataContractStore();

            dataContractStore.Add("item", new
            {
                Items = new[] { dataContractStore.Get("item") }
            });

            new DataContractValidator().Validate(typeof(Item), dataContractStore.Get("item"));
        }

        private class Item
        {
            public IEnumerable<Item> Items { get; set; }
        }
    }
}