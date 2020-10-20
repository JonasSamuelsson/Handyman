using System;

namespace Handyman.Tools.Docs.Utils
{
    public class Logger : ILogger
    {
        public IDisposable CreateScope(string message)
        {
            return new Disposable();
        }

        public IDisposable UsePrefix(string prefix)
        {
            return new Disposable();
        }

        public void WriteError(string message)
        {
            // todo
        }

        public void WriteInfo(string message)
        {
            // todo
        }
    }
}