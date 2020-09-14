using Shouldly;
using System.Dynamic;
using Xunit;

namespace Handyman.DuckTyping.Tests
{
    public class GeneratedDuckTypedObjectTests
    {
        [Fact]
        public void ShouldGenerateClassForInterface()
        {
            var expando = new ExpandoObject();

            var o1 = DuckTyped.Object<IObject1>(expando, x => x.Text = "success");

            var o2 = DuckTyped.Object<IObject2>(expando);

            o2.Text.ShouldBe("success");

            var o3 = DuckTyped.Object<IObject2>(o1);

            o3.Text.ShouldBe("success");
        }

        [DuckTypedObjectContract]
        public interface IObject1
        {
            string Text { get; set; }
        }

        [DuckTypedObjectContract]
        public interface IObject2
        {
            string Text { get; set; }
        }
    }
}
