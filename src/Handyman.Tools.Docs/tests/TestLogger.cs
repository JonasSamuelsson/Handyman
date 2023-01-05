using Handyman.Tools.Docs.Shared;
using System;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.Tests;

public class TestLogger : Logger
{
    public Func<LogLevel, string> LogLevelFormatter { get; set; } = _ => string.Empty;
    public List<string> Output { get; } = new();

    protected override string Format(string message, LogLevel logLevel)
    {
        var logLevelFormat = LogLevelFormatter.Invoke(logLevel);
        return $"{logLevelFormat}{message}";
    }

    protected override void Write(string line)
    {
        Output.Add(line);
    }
}