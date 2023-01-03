using Handyman.Tools.Docs.Shared;

namespace Handyman.Tools.Docs.ImportCodeBlocks;

public class CodeBlockAttributes
{
    public string? Language { get; set; }
    public string? Id { get; set; }
    [AttributeName("Lines")] public LinesSpec? LinesSpec { get; set; }
    public string Source { get; set; }
}