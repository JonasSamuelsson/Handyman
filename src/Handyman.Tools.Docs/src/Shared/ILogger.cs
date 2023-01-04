using System;

namespace Handyman.Tools.Docs.Shared;

public interface ILogger
{
    IDisposable Scope(string scope);

    void WriteLine(LogLevel logLevel, string message);

    void WriteDebugLine(string message)
    {
        WriteLine(LogLevel.Debug, message);
    }

    void WriteInfoLine(string message)
    {
        WriteLine(LogLevel.Info, message);
    }

    void WriteErrorLine(string message)
    {
        WriteLine(LogLevel.Error, message);
    }
}