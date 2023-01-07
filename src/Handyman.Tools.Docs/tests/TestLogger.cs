using Handyman.Tools.Docs.Shared;
using System;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.Tests;

public class TestLogger : Logger
{
    public Func<LogLineType, string> LogLevelFormatter { get; set; } = _ => string.Empty;
    public List<string> Output { get; } = new();

    protected override void WriteLine(LogLineType logLineType, string line)
    {
        Output.Add(line);
    }
}