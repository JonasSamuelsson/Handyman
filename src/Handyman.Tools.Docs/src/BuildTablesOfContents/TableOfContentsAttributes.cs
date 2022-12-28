using System.Collections.Generic;

namespace Handyman.Tools.Docs.BuildTablesOfContents;

public class TableOfContentsAttributes
{
    public IReadOnlyCollection<int> Levels { get; set; } = new[] { 1, 2, 3, 4, 5, 6 };
    public ListType ListType { get; set; } = ListType.Unordered;
    public string SourcePath { get; set; }
}