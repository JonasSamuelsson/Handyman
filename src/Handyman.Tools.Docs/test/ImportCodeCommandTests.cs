using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class ImportCodeCommandTests
    {
        [Fact]
        public async Task ShouldImportCode()
        {
            var fileSystem = new MockFileSystem();

            fileSystem.AddFile(@"x:\dir\readme.md", new[]
            {
                "1",
                "<!-- <handyman-docs:import-code src=\"a.cs\" /> -->",
                "2",
                "<!-- <handyman-docs:import-code src=\"b.txt\" lines=\"2\" /> -->",
                "3",
                "<!-- <handyman-docs:import-code src=\"c.ps1\" lines=\"2-4\"> -->",
                "4",
                "<!-- </handyman-docs:import-code> -->",
                "5",
                //"<!-- <handyman-docs:import-code src=\"a.cs\" language=\"test\" /> -->",
                //"6"
            });
            fileSystem.AddFile(@"x:\dir\a.cs", new[]
            {
                "public class A { }"
            });
            fileSystem.AddFile(@"x:\dir\b.txt", new[]
            {
                "first",
                "second",
                "third"
            });
            fileSystem.AddFile(@"x:\dir\c.ps1", new[]
            {
                "write-host 1",
                "write-host 2",
                "write-host 3",
                "write-host 4",
                "write-host 5"
            });

            var task = Program.Run(services =>
            {

                services.Replace(new ServiceDescriptor(typeof(IFileSystem), fileSystem));
            }, new[] { "import-code", @"x:\dir" });

            (await task).ShouldBe(0);

            fileSystem.GetFile(@"x:\dir\readme.md").GetLines().ShouldBe(new[]
            {
                "1",
                "<!-- <handyman-docs:import-code src=\"a.cs\"> -->",
                "```csharp",
                "public class A { }",
                "```",
                "<!-- </handyman-docs:import-code> -->",
                "2",
                "<!-- <handyman-docs:import-code src=\"b.txt\" lines=\"2\"> -->",
                "```txt",
                "second",
                "```",
                "<!-- </handyman-docs:import-code> -->",
                "3",
                "<!-- <handyman-docs:import-code src=\"c.ps1\" lines=\"2-4\"> -->",
                "```powershell",
                "write-host 2",
                "write-host 3",
                "write-host 4",
                "```",
                "<!-- </handyman-docs:import-code> -->",
                "5",
                "" // System.IO.Abstractions.TestHelpers adds an extra blank line when using File.WriteAllLines
            });
        }
    }
}