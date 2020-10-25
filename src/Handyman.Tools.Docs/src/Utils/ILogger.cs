using System;

namespace Handyman.Tools.Docs.Utils
{
    public interface ILogger
    {
        IDisposable CreateScope(string scope);
        IDisposable UsePrefix(string prefix);
        void WriteError(string message);
        void WriteInfo(string message);
    }
}