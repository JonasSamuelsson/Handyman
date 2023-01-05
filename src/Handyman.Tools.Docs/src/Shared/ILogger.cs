using System;

namespace Handyman.Tools.Docs.Shared;

public interface ILogger
{
    IDisposable Scope(string scope);

    void Write(LogLevel logLevel, string message);

    void WriteDebug(string message)
    {
        Write(LogLevel.Debug, message);
    }

    void WriteInfo(string message)
    {
        Write(LogLevel.Info, message);
    }

    void WriteError(string message)
    {
        Write(LogLevel.Error, message);
    }
}