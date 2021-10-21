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
   Enum = new Enum(new Dictionary<int, string>
   {
      { 0, ""Zero"" },
      { 1, ""One"" }
   })
   {
      Flags = false,
      Nullable = true
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
            public IEnumerable<Child> ChildCollection { get; set; }
            public IDictionary<int, Child> ChildDictionary { get; set; }
            public IEnumerable<string> StringCollection { get; set; }
            public IDictionary<int, string> StringDictionary { get; set; }
        }

        private class Child
        {
            public decimal? Number { get; set; }
            public string Text { get; set; }
        }

        private enum MyEnum
        {
            Zero = 0,
            One = 1
        }
    }
}