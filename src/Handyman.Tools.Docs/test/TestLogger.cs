using System;
using Handyman.Tools.Docs.Utils;

namespace Handyman.Tools.Docs.Tests
{
    internal class TestLogger : ILogger
    {
        public void Log(string message)
        {
            // todo
        }

        public IDisposable UsePrefix(string prefix)
        {
            return new Disposable();
        }
    }
}