using Handyman.Tools.Docs.Shared;

namespace Handyman.Tools.Docs.ImportCodeBlocks;

public class CodeBlockSourceAttributes
{
    public string Id { get; set; }
    [AttributeName("Lines")] public LinesSpec? LinesSpec { get; set; }
}