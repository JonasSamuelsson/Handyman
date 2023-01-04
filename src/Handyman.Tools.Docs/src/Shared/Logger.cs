using Spectre.Console;
using System;

namespace Handyman.Tools.Docs.Shared;

public abstract class LoggerBase : ILogger
{
    private string _indentation = string.Empty;

    public IDisposable Scope(string scope)
    {
        var s = _indentation;
        _indentation += " ";
        return new Disposable(() => _indentation = s);
    }

    public virtual void WriteLine(LogLevel logLevel, string message)
    {
        WriteLine(message);
    }

    protected abstract void WriteLine(string message);
}

public class Logger : LoggerBase
{
    private readonly IAnsiConsole _console;

    public Logger(IAnsiConsole console)
    {
        _console = console;
    }

    public override void WriteLine(LogLevel logLevel, string message)
    {
        var format = GetFormat(logLevel);

        var prefix = format.Length == 0 ? string.Empty : $"[{format}]";
        var postfix = format.Length == 0 ? string.Empty : "[/]";

        base.WriteLine(logLevel, $"{prefix}{message}{postfix}");
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

    protected override void WriteLine(string message)
    {
        _console.WriteLine(message);
    }
}