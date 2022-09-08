using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests.Docs
{
    public class Dictionaries
    {
        class MyClass
        {
            public Dictionary<int, string> DictionaryWithSimpleValue1 { get; set; }
            public Dictionary<int, string> DictionaryWithSimpleValue2 { get; set; }
            public Dictionary<int, ComplexClass> DictionaryWithComplexValue { get; set; }
        }

        class ComplexClass
        {
            public string Text { get; set; }
        }

        [Fact]
        public void Validate()
        {
            var type = typeof(MyClass);

            var dataContract = new
            {
                // there are multiple ways to declare dictionaries with values of simple types
                DictionaryWithSimpleValue1 = typeof(Dictionary<int, string>),
                DictionaryWithSimpleValue2 = new Dictionary<int>
                {
                    typeof(string)
                },
                DictionaryWithComplexValue = new Dictionary<int>
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