using System;

namespace Handyman.Tools.Docs.Shared;

public interface ILogger
{
    IDisposable Scope(string scope);

    void WriteDebug(string message);
    void WriteError(string message);
    Verbosity Verbosity { get; set; }
}