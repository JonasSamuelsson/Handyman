using System;

namespace Handyman.Tools.Docs.Shared;

public class Disposable : IDisposable
{
    private readonly Action _action;

    public Disposable(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action.Invoke();
    }
}