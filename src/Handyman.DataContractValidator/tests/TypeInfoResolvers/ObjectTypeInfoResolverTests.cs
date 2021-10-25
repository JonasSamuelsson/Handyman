using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.TypeInfoResolvers;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests.TypeInfoResolvers
{
    public class ObjectTypeInfoResolverTests
    {
        [Theory, MemberData(nameof(ShouldResolveObjectTypeInfoParams))]
        public void ShouldResolveObjectTypeInfo(object o)
        {
            var parent = new TypeInfoResolverContext().GetTypeInfo(o).ShouldBeOfType<ObjectTypeInfo>();

            parent.Properties.Count().ShouldBe(2);
            parent.Properties.Single(x => x.Name == "Number").Value.Name.ShouldBe("int");
            var child = parent.Properties.Single(x => x.Name == "Child").Value.ShouldBeOfType<ObjectTypeInfo>();
            child.Properties.Count().ShouldBe(1);
            child.Properties.Single(x => x.Name == "Text").Value.Name.ShouldBe("string");
        }

        public static IEnumerable<object[]> ShouldResolveObjectTypeInfoParams()
        {
            return new[]
            {
                new object[] { new Parent() },
                new object[] { typeof(Parent) },
                new object[]
                {
                    new
                    {
                        Child = new { Text = typeof(string) },
                        Number = typeof(int)
                    }
                }
            };
        }

        public class Parent
        {
            public Child Child { get; set; } = null!;
            public int Number { get; set; }
        }

        public class Child
        {
            public string Text { get; set; } = null!;
        }

        [Fact]
        public void ShouldThrowIfTypeIsAnonymous()
        {
            var type = new { }.GetType();
            var dataContract = new { };

            Should.Throw<NotSupportedException>(() => new DataContractValidator().Validate(type, dataContract))
                .Message.ShouldBe("Can't handle type of anonymous object.");
        }
    }
}