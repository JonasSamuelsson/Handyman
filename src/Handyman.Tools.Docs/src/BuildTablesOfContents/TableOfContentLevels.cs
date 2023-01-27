using System.Collections.Generic;

namespace Handyman.Tools.Docs.BuildTablesOfContents;

public class TableOfContentLevels
{
    public bool Current { get; set; }
    public int CurrentAdditionalLevels { get; set; }
    public IReadOnlyCollection<int> ExplicitLevels { get; set; } = new int[] { 1, 2, 3, 4, 5, 6 };
}