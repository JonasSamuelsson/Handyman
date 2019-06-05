using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class RecursiveObjectGraphTests
    {
        [Fact]
        public void ShouldPassWhenRecursiveObjectMatch()
        {
            var type = typeof(ActualRecursiveObject);
            var dataContract = typeof(ExpectedRecursiveObject);

            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        [Fact]
        public void ShouldFailWhenRecursiveObjectDoNotMatch()
        {
            var type = typeof(ActualRecursiveObject);
            var dataContract = new
            {
                Text = typeof(string),
                Child = new
                {
                    Text = typeof(string),
                    Child = new
                    {
                        Text = typeof(string),
                        Child = new
                        {
                            Text = typeof(string)
                        }
                    }
                }
            };

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Child.Child.Child.Child : unexpected property." });
        }

        [Fact]
        public void ShouldValidateRecursiveObjectWithRegisteredDataContract()
        {
            var validator = new DataContractValidator();

            validator.AddDataContract("x", new
            {
                Text = typeof(string),
                Child = validator.GetDataContract("x")
            });

            validator.Validate(typeof(ActualRecursiveObject), validator.GetDataContract("x"));
        }

        private class ActualRecursiveObject
        {
            public string Text { get; set; }
            public ActualRecursiveObject Child { get; set; }
        }

        private class ExpectedRecursiveObject
        {
            public string Text { get; set; }
            public ExpectedRecursiveObject Child { get; set; }
        }

        [Fact]
        public void ShouldPassWhenRecursiveCollectionMatch()
        {
            var type = typeof(ActualRecursiveCollection);
            var dataContract = typeof(ExpectedRecursiveCollection);

            new DataContractValidator().Validate(type, dataContract, out _).ShouldBeTrue();
        }

        [Fact]
        public void ShouldFailWhenRecursiveCollectionDoNotMatch()
        {
            var type = typeof(ActualRecursiveCollection);
            var dataContract = new
            {
                Text = typeof(string),
                Children = new[]
                {
                    new
                    {
                        Text = typeof(string),
                        Children = new[]
                        {
                            new
                            {
                                Text = typeof(string),
                                Children = new[]
                                {
                                    new
                                    {
                                        Text = typeof(string)
                                    }
                                }
                            }
                        }
                    }
                }
            };

            new DataContractValidator().Validate(type, dataContract, out var errors).ShouldBeFalse();

            errors.ShouldBe(new[] { "Children.Item.Children.Item.Children.Item.Children : unexpected property." });
        }

        [Fact]
        public void ShouldValidateRecursiveCollectionWithRegisteredDataContract()
        {
            var validator = new DataContractValidator();

            validator.AddDataContract("node", new
            {
                Text = typeof(string),
                Children = new[] {validator.GetDataContract("node")}
            });

            validator.Validate(typeof(ActualRecursiveCollection), validator.GetDataContract("node"));
        }

        private class ActualRecursiveCollection
        {
            public string Text { get; set; }
            public IEnumerable<ActualRecursiveCollection> Children { get; set; }
        }

        private class ExpectedRecursiveCollection
        {
            public string Text { get; set; }
            public IEnumerable<ExpectedRecursiveCollection> Children { get; set; }
        }
    }
}