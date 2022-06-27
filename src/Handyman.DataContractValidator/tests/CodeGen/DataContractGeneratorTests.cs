using Shouldly;
using System;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen
{
    public class DataContractGeneratorTests
    {
        [Fact]
        public void ShouldThrowOnRecursiveType()
        {
            var dataContractGenerator = new DataContractGenerator();

            Should.Throw<InvalidOperationException>(() => dataContractGenerator.GenerateFor<Item>())
                .Message.ShouldBe("DataContractGenerator does not support recursive types.");
        }

        private class Item
        {
            public Item Child { get; set; }
        }
    }
}