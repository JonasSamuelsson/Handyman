using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Docs.Shared;

public abstract class Logger : ILogger
{
    private readonly List<string> _allScopes = new();
    private readonly List<string> _scopesToPrint = new();

    public Verbosity Verbosity { get; set; } = Verbosity.Normal;

    public IDisposable Scope(string scope)
    {
        scope = GetIndentation() + scope;

        _allScopes.Add(scope);
        _scopesToPrint.Add(scope);

        return new Disposable(() =>
        {
            _allScopes.RemoveAt(_allScopes.Count - 1);
            if (_scopesToPrint.Count == 0) return;
            _scopesToPrint.RemoveAt(_scopesToPrint.Count - 1);
        });
    }

    public virtual void Write(LogLevel logLevel, string message)
    {
        if (!ShouldWrite(logLevel))
            return;

        foreach (var scope in _scopesToPrint)
        {
            Write(scope);
        }

        _scopesToPrint.Clear();

        Write(GetIndentation() + Format(message, logLevel));
    }

    private bool ShouldWrite(LogLevel logLevel)
    {
        if (Verbosity == Verbosity.Quiet)
            return false;

        if (Verbosity == Verbosity.Diagnostics)
            return true;

        return logLevel == LogLevel.Error || logLevel == LogLevel.Info;
    }

    protected abstract string Format(string message, LogLevel logLevel);

    protected abstract void Write(string line);

    private string GetIndentation()
    {
        return new string(' ', _allScopes.Count * 2);
    }

    private class Scopes
    {
        public List<string> Items { get; } = new();

        public void Push(string scope)
        {
            Items.Add(scope);
        }

        public bool Pop(string scope)
        {
            if (Items.Count == 0)
                return false;

            if (Items.Last() != scope)
                return false;

            Items.RemoveAt(Items.Count - 1);
            return true;
        }
    }
}

public enum Verbosity
{
    Quiet = 0,
    Normal = 1,
    Diagnostics = 2
}