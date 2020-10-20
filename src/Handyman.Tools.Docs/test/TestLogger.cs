using Handyman.Tools.Docs.Utils;
using System;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.Tests
{
    internal class TestLogger : ILogger
    {
        public List<string> Messages { get;  }=new List<string>();

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