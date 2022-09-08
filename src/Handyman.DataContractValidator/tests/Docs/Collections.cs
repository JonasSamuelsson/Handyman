using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Docs
{
    public class Collections
    {
        class MyClass
        {
            public IEnumerable<int> SimpleCollection1 { get; set; }
            public IEnumerable<int> SimpleCollection2 { get; set; }
            public IEnumerable<Child> ComplexCollection { get; set; }
        }

        class Child
        {
            public string Text { get; set; }
        }

        [Fact]
        public void Validate()
        {
            var type = typeof(MyClass);

            var dataContract = new
            {
                // there are multiple ways to declare collections of simple types
                SimpleCollection1 = typeof(IEnumerable<int>),
                SimpleCollection2 = new[] { typeof(int) },
                ComplexCollection = new[]
                {
                    new
                    {
                        Text = typeof(string)
                    }
                }
            };

            new DataContractValidator().Validate(type, dataContract);
        }
    }
}