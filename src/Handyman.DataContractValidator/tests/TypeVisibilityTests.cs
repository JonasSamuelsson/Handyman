using System;
using System.Linq;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class TypeVisibilityTests
    {
        [Fact]
        public void AllPublicTypesShouldBelongToRootNamespace()
        {
            var ns = "Handyman.DataContractValidator";

            var types = typeof(DataContractValidator).Assembly
                .GetTypes()
                .Where(x => x.IsPublic && x.Namespace != "Handyman.DataContractValidator")
                .Select(x => x.FullName)
                .OrderBy(x => x)
                .ToList();

            if (!types.Any())
                return;

            var strings = new[] { $"These types should not be public or move to namespace {ns}." }
                .Concat(types);

            throw new Exception(string.Join(Environment.NewLine, strings));
        }
    }
}