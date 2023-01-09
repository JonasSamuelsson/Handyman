using Handyman.Tools.Docs.ImportCodeBlocks;
using Handyman.Tools.Docs.Shared;

namespace Handyman.Tools.Docs.ImportContent;

public class ContentSourceAttributes
{
    public string Id { get; set; }
    [AttributeName("Lines")] public LinesSpec? LinesSpec { get; set; }
}