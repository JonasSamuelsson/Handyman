using System;

namespace Handyman.Tools.Docs.Shared;

public interface ILogger
{
    void Scope(string scope, Action action);
    T Scope<T>(string scope, Func<T> action);

    IDisposable Scope(string scope);

    void WriteDebug(string message);
    void WriteError(string message);
    Verbosity Verbosity { get; set; }
}