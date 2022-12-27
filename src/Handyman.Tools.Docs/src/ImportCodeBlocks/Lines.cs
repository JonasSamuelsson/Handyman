namespace Handyman.Tools.Docs.ImportCodeBlocks;

public class Lines
{
    public int Count { get; set; }
    public int FromNumber { get; set; }

    public int FromIndex => FromNumber - 1;
}