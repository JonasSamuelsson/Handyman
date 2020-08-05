using System.Collections.Generic;
using FluentAssertions;
using Handyman.Tools.Outdated.Analyze.DotnetListPackage;
using Handyman.Tools.Outdated.Model;
using System.Linq;
using Xunit;

namespace Handyman.Tools.Outdated.Tests
{
    public class DotnetListPackageResultReaderTests
    {
        [Fact]
        public void ShouldRead()
        {
            var outputs = new[]
            {
                new[]
                {
                    "Project `Sample` has the following updates to its packages",
                    "   [netstandard2.0]:",
                    "   Top-level Package      Requested   Resolved   Latest",
                    "   > Alpha                1.0.0       1.0.0      1.1.0",
                    "   > Beta                 1.1.0       1.1.0      Not found at the sources",
                    "",
                    "   Transitive Package      Resolved   Latest",
                    "   > Gamma                 1.0.0      2.0.0"
                },
                new[]
                {
                    "Project `Sample` has the following updates to its packages",
                    "   [netstandard2.0]:",
                    "   Top-level Package      Requested   Resolved   Latest",
                    "   > Alpha                1.0.0       1.0.0      1.0.2 (D)"
                },
                new[]
                {
                    "Project `Sample` has the following deprecated packages",
                    "   [netstandard2.0]:",
                    "   Top-level Package      Resolved   Reason(s)   Alternative",
                    "   > Alpha                1.0.0      Foo",
                    "",
                    "   Transitive Package      Resolved   Reason(s)   Alternative",
                    "   > Gamma                 1.0.0      Bar         Something else"
                },
                new[]
                {
                    "Project `Sample` has the following updates to its packages",
                    "   [netcoreapp3.1]:",
                    "   Top-level Package      Requested   Resolved   Latest",
                    "   > Alpha                1.0.0       1.0.0      1.0.1"
                }
            };

            var reader = new ResultReader();
            var targetFrameworks = new List<TargetFramework>();

            foreach (var output in outputs)
            {
                reader.Read(output, targetFrameworks);
            }

            targetFrameworks.Should().HaveCount(2);

            var framework = targetFrameworks.Single(x => x.Name == "netstandard2.0");

            framework.Packages.Should().HaveCount(3);

            var package = framework.Packages.Single(x => x.Name == "Alpha");
            package.CurrentVersion.Should().Be("1.0.0");
            package.Deprecation.Alternative.Should().BeEmpty();
            package.Deprecation.IsDeprecated.Should().BeTrue();
            package.Deprecation.Reason.Should().Be("Foo");
            package.Info.Should().BeEmpty();
            package.IsTransitive.Should().BeFalse();
            package.Updates.Should().HaveCount(2);

            var update = package.Updates[UpdateSeverity.Minor];
            update.Info.Should().BeEmpty();
            update.Version.Should().Be("1.1.0");

            update = package.Updates[UpdateSeverity.Patch];
            update.Info.Should().Be("Deprecated");
            update.Version.Should().Be("1.0.2");

            package = framework.Packages.Single(x => x.Name == "Beta");
            package.CurrentVersion.Should().Be("1.1.0");
            package.Deprecation.Alternative.Should().BeEmpty();
            package.Deprecation.IsDeprecated.Should().BeFalse();
            package.Deprecation.Reason.Should().BeEmpty();
            package.Info.Should().Be("Not found at the sources");
            package.IsTransitive.Should().BeFalse();
            package.Updates.Should().HaveCount(0);

            package = framework.Packages.Single(x => x.Name == "Gamma");
            package.CurrentVersion.Should().Be("1.0.0");
            package.Deprecation.Alternative.Should().Be("Something else");
            package.Deprecation.IsDeprecated.Should().BeTrue();
            package.Deprecation.Reason.Should().Be("Bar");
            package.Info.Should().BeEmpty();
            package.IsTransitive.Should().BeTrue();
            package.Updates.Should().HaveCount(1);

            update = package.Updates[UpdateSeverity.Major];
            update.Info.Should().BeEmpty();
            update.Version.Should().Be("2.0.0");

            framework = targetFrameworks.Single(x => x.Name == "netcoreapp3.1");

            package = framework.Packages.Single();
            package.CurrentVersion.Should().Be("1.0.0");
            package.Deprecation.Alternative.Should().BeEmpty();
            package.Deprecation.IsDeprecated.Should().BeFalse();
            package.Deprecation.Reason.Should().BeEmpty();
            package.Name.Should().Be("Alpha");
            package.Info.Should().BeEmpty();
            package.IsTransitive.Should().BeFalse();
            package.Updates.Should().HaveCount(1);

            update = package.Updates[UpdateSeverity.Patch];
            update.Info.Should().BeEmpty();
            update.Version.Should().Be("1.0.1");
        }
    }
}