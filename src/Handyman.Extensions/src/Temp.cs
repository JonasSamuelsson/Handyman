using System;

namespace Handyman.Extensions;

public class Temp : IDisposable
{
    private Action _restore;

    public Temp(Action modify, Action restore)
    {
        modify();
        _restore = restore;
    }

    public void Dispose()
    {
        var restore = _restore;
        _restore = null;
        restore();
    }
}