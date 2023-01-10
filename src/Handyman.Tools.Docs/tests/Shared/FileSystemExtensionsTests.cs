using Handyman.Tools.Docs.Shared;
using Shouldly;
using System;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Handyman.Tools.Docs.Tests.Shared;

public class FileSystemExtensionsTests
{
    [Fact]
    public void ShouldGetGitRepoDirectory()
    {
        var fileSystem = new MockFileSystem();

        fileSystem.Directory.CreateDirectory(@"C:\foo");
        fileSystem.Directory.CreateDirectory(@"C:\foo\.git");
        fileSystem.Directory.CreateDirectory(@"C:\foo\bar");

        fileSystem.GetGitRepoDirectory(@"C:\foo\bar").ShouldBe(@"C:\foo");
        fileSystem.GetGitRepoDirectory(@"C:\foo\bar\file.txt").ShouldBe(@"C:\foo");
    }

    [Fact]
    public void ShouldThrowIfGetGitRepoDirectoryIsCalledWithPathNotFullyQualified()
    {
        var fileSystem = new MockFileSystem();

        Should.Throw<ArgumentException>(() => fileSystem.GetGitRepoDirectory("file.txt"))
            .Message.ShouldBe("Provided path is not fully qualified.");
    }

    [Fact]
    public void ShouldThrowIfGitRepoDirectoryIsNotFound()
    {
        var fileSystem = new MockFileSystem();

        Should.Throw<AppException>(() => fileSystem.GetGitRepoDirectory(@"C:\foo\file.txt"))
            .Message.ShouldBe("Parent git repo directory not found.");
    }

    [Fact]
    public void ShouldGetMarkdownFilePaths()
    {
        var fileSystem = new MockFileSystem();

        fileSystem.AddFile(@"C:\one.md", new string[] { });
        fileSystem.AddFile(@"C:\temp\two.md", new string[] { });
        fileSystem.AddFile(@"C:\temp\thre.txt", new string[] { });
        fileSystem.AddFile(@"C:\temp\child\four.md", new string[] { });

        fileSystem.GetMarkdownFilePaths(@"C:\temp").ShouldBe(new[]
        {
            @"C:\temp\two.md",
            @"C:\temp\child\four.md"
        });
    }

    [Theory]
    [InlineData(@"C:\a\b\file.txt", @"c\d\index.html", @"C:\a\b\c\d\index.html")]
    [InlineData(@"C:\a\b\file.txt", @".\c\d\index.html", @"C:\a\b\c\d\index.html")]
    [InlineData(@"C:\a\b\file.txt", @"..\c\d\index.html", @"C:\a\c\d\index.html")]
    [InlineData(@"C:\a\b\file.txt", @"/index.html", @"C:\a\index.html")]
    [InlineData(@"C:\a\b\file.txt", @"\index.html", @"C:\a\index.html")]
    public void ShouldConstructPathRelativeToFile(string referencePath, string relativePath, string expected)
    {
        var fileSystem = new MockFileSystem();

        fileSystem.Directory.CreateDirectory(@"C:\a");
        fileSystem.Directory.CreateDirectory(@"C:\a\.git");

        fileSystem.ConstructPathRelativeToFile(referencePath, relativePath).ShouldBe(expected);
    }

    [Fact]
    public void ConstructPathRelativeToFileShouldThrowIfReferencePathIsNotFullyQualified()
    {
        Should.Throw<Exception>(() => new MockFileSystem().ConstructPathRelativeToFile("file.txt", null))
            .Message.ShouldBe("Reference path is not fully qualified.");
    }

    [Fact]
    public void ConstructPathRelativeToFileShouldThrowIfRelativePathIsFullyQualified()
    {
        Should.Throw<Exception>(() => new MockFileSystem().ConstructPathRelativeToFile(@"C:\temp\file.txt", @"C:\temp\index.html"))
            .Message.ShouldBe("Relative path is fully qualified.");
    }

    [Fact]
    public void ConstructPathRelativeToFileShouldThrowIfTheRelativePathBacktracksToFar()
    {
        Should.Throw<Exception>(() => new MockFileSystem().ConstructPathRelativeToFile(@"C:\file.txt", @"..\..\temp\index.html"))
            .Message.ShouldBe("Resulting path is outside of the root directory.");
    }
}