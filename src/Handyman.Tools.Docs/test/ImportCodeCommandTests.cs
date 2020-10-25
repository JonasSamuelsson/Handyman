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
                "<!-- <handyman-docs:code-block source=\"a.cs\" /> -->",
                "2",
                "<!-- <handyman-docs:code-block source=\"b.txt\" lines=\"2\" /> -->",
                "3",
                "<!-- <handyman-docs:code-block source=\"c.ps1\" lines=\"2-4\"> -->",
                "4",
                "<!-- </handyman-docs:code-block> -->",
                "5",
                //"<!-- <handyman-docs:code-block source=\"a.cs\" language=\"test\" /> -->",
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
            }, new[] { "code-blocks", @"x:\dir" });

            (await task).ShouldBe(0);

            fileSystem.GetFile(@"x:\dir\readme.md").GetLines().ShouldBe(new[]
            {
                "1",
                "<!-- <handyman-docs:code-block source=\"a.cs\"> -->",
                "```csharp",
                "public class A { }",
                "```",
                "<!-- </handyman-docs:code-block> -->",
                "2",
                "<!-- <handyman-docs:code-block source=\"b.txt\" lines=\"2\"> -->",
                "```txt",
                "second",
                "```",
                "<!-- </handyman-docs:code-block> -->",
                "3",
                "<!-- <handyman-docs:code-block source=\"c.ps1\" lines=\"2-4\"> -->",
                "```powershell",
                "write-host 2",
                "write-host 3",
                "write-host 4",
                "```",
                "<!-- </handyman-docs:code-block> -->",
                "5",
                "" // System.IO.Abstractions.TestHelpers adds an extra blank line when using File.WriteAllLines
            });
        }
    }
}