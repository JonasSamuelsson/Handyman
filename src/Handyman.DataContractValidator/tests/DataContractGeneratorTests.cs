using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class DataContractGeneratorTests
    {
        [Fact]
        public void ShouldGenerateDataContract()
        {
            new DataContractGenerator()
                .GenerateFor<Root>()
                .ShouldBe(@"new
{
   Id = typeof(int),
   Enum = new Enum
   {
      Flags = false,
      Nullable = true,
      Values =
      {
         { 0, ""Zero"" },
         { 1, ""One"" }
      }
   },
   ChildCollection = new []
   {
      new
      {
         Number = typeof(decimal?),
         Text = typeof(string)
      }
   },
   ChildDictionary = new Dictionary<int>
   {
      new
      {
         Number = typeof(decimal?),
         Text = typeof(string)
      }
   },
   StringCollection = new [] { typeof(string) },
   StringDictionary = new Dictionary<int> { typeof(string) }
}");
        }

        private class Root
        {
            public int Id { get; set; }
            public MyEnum? Enum { get; set; }
            public IEnumerable<Child> ChildCollection { get; set; } = null!;
            public IDictionary<int, Child> ChildDictionary { get; set; } = null!;
            public IEnumerable<string> StringCollection { get; set; } = null!;
            public IDictionary<int, string> StringDictionary { get; set; } = null!;
        }

        private class Child
        {
            public decimal? Number { get; set; }
            public string Text { get; set; } = null!;
        }

        private enum MyEnum
        {
            Zero = 0,
            One = 1
        }
    }
}