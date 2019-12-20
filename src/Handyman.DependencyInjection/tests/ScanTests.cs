using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class ScanTests
    {
        [Fact]
        public void ShouldScan()
        {
            var convention = new Convention();

            new ServiceCollection().Scan(_ =>
            {
                _.AssemblyContainingTypeOf(this);
                _.Where(x => x.DeclaringType == typeof(ScanTests));
                _.Using(convention);
            });

            convention.ProcessedTypes.ShouldBe(new[] { typeof(Convention) });
        }

        private class Convention : IServiceConfigurationConvention
        {
            public List<Type> ProcessedTypes { get; } = new List<Type>();

            public void Execute(IReadOnlyCollection<Type> types, IServiceCollection services)
            {
                ProcessedTypes.AddRange(types);
            }
        }
    }
}