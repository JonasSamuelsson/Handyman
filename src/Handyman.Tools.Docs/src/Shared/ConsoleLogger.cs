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

    protected override string Format(string message, LogLevel logLevel)
    {
        var format = GetFormat(logLevel);

        var prefix = format.Length == 0 ? string.Empty : $"[{format}]";
        var postfix = format.Length == 0 ? string.Empty : "[/]";

        return $"{prefix}{message}{postfix}";
    }

    private static string GetFormat(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => "purple",
            LogLevel.Info => "",
            LogLevel.Error => "red",
            _ => throw new Exception($"Unhandled log level '{logLevel}'.")
        };
    }

    protected override void Write(string line)
    {
        _console.WriteLine(line);
    }
}