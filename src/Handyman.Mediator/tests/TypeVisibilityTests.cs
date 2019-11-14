using System;
using System.Linq;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class TypeVisibilityTests
    {
        [Fact]
        public void TypesWithFullNameContainingInternalShouldNotBePublic()
        {
            var errors = typeof(IMediator).Assembly.GetTypes()
                .Where(x => x.IsPublic)
                .Where(x => x.FullName.ToLowerInvariant().Contains("internal"))
                .Select(x => x.FullName)
                .ToList();

            if (!errors.Any())
                return;

            throw new Exception(string.Join(Environment.NewLine, errors));
        }

        [Fact]
        public void NestedTypesShouldNotBePublic()
        {
            var errors = typeof(IMediator).Assembly.GetTypes()
                .Where(x => x.IsNestedPublic)
                .Select(x => x.FullName)
                .ToList();

            if (!errors.Any())
                return;

            throw new Exception(string.Join(Environment.NewLine, errors));
        }
    }
}
