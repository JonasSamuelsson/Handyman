using Xunit;

namespace Handyman.DataContractValidator.Tests.Docs
{
    public class NullableReferenceTypes
    {
        class MyClass
        {
            public string? Text { get; set; }
        }

        [Fact]
        public void Validate()
        {
            var type = typeof(MyClass);

            var dataContract = new
            {
                Text = new CanBeNull(typeof(string))
            };

            new DataContractValidator().Validate(type, dataContract);
        }
    }
}