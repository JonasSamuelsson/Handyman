using Xunit;

namespace Handyman.DataContractValidator.Tests.Docs
{
    public class Objects
    {
        class MyParentClass
        {
            public MyChildClass Child { get; set; }
            public bool Flag { get; set; }
            public int Number { get; set; }
        }

        class MyChildClass
        {
            public string Text { get; set; }
        }

        [Fact]
        public void Validate()
        {
            var type = typeof(MyParentClass);

            var dataContract = new
            {
                Child = new
                {
                    Text = typeof(string)
                },
                Flag = typeof(bool),
                Number = typeof(int)
            };

            new DataContractValidator().Validate(type, dataContract);
        }
    }
}