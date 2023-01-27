namespace Handyman.Tools.Docs.BuildTablesOfContents;

public class TableOfContentsAttributes
{
    public TableOfContentLevels Levels { get; set; } = new();
    public ListType ListType { get; set; } = ListType.Unordered;
    public string SourcePath { get; set; }
}