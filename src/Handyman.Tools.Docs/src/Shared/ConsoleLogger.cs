using Spectre.Console;
using System;

namespace Handyman.Tools.Docs.Shared;

public class ConsoleLogger : Logger
{
    private readonly IAnsiConsole _console;

    public ConsoleLogger(IAnsiConsole console)
    {
        _console = console;
    }

    protected override void WriteLine(LogLineType logLineType, string line)
    {
        var format = GetFormat(logLineType);

        if (string.IsNullOrWhiteSpace(format))
        {
            _console.WriteLine(line);
            return;
        }

        _console.Markup(format);
        _console.Write(line);
        _console.MarkupLine("[/]");
    }

    private static string GetFormat(LogLineType logLineType)
    {
        return logLineType switch
        {
            LogLineType.Debug => "[purple]",
            LogLineType.Error => "[red]",
            LogLineType.Scope => "",
            _ => throw new Exception($"Unhandled log level '{logLineType}'.")
        };
    }
}