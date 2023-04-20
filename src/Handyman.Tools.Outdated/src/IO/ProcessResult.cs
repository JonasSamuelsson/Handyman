using System.Collections.Generic;

namespace Handyman.Tools.Outdated.IO;

public class ProcessResult
{
    public int ExitCode { get; set; }
    public IReadOnlyList<string> StandardError { get; set; }
    public IReadOnlyList<string> StandardOutput { get; set; }
}