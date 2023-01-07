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

    public void WriteDebug(string message)
    {
        Write(LogLineType.Debug, message);
    }

    public void WriteError(string message)
    {
        Write(LogLineType.Error, message);
    }

    public virtual void Write(LogLineType logLineType, string message)
    {
        if (!ShouldWrite(logLineType))
            return;

        foreach (var scope in _scopesToPrint)
        {
            WriteLine(LogLineType.Scope, scope);
        }

        _scopesToPrint.Clear();

        WriteLine(LogLineType.Scope, GetIndentation() + message);
    }

    private bool ShouldWrite(LogLineType logLineType)
    {
        if (Verbosity == Verbosity.Quiet)
            return false;

        if (Verbosity == Verbosity.Diagnostics)
            return true;

        return logLineType == LogLineType.Error;
    }

    protected abstract void WriteLine(LogLineType logLineType, string line);

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