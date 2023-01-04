using Handyman.Tools.Docs.Shared;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.Tests;

public class TestLogger : LoggerBase
{
    public List<string> Entries { get; } = new();

    protected override void WriteLine(string message)
    {
        Entries.Add(message);
    }
}