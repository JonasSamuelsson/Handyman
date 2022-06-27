using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Validation
{
    public class RecursiveTypeValidationTests
    {
        [Fact]
        public void ShouldValidateRecursiveTypes()
        {
            var dataContractValidator = new DataContractValidator();

            dataContractValidator.DataContracts.Add("item", new
            {
                Items = new[] { dataContractValidator.DataContracts.Get("item") }
            });

            dataContractValidator.Validate(typeof(Item), dataContractValidator.DataContracts.Get("item"));
        }

        private class Item
        {
            public IEnumerable<Item> Items { get; set; }
        }
    }
}