using System;

namespace Handyman.Tools.Docs.Utils
{
    public interface ILogger
    {
        void Log(string message);
        IDisposable UsePrefix(string prefix);
    }
}