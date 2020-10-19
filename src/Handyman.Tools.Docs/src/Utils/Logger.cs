using System;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public class Logger : ILogger
    {
        public List<string> Messages { get; } = new List<string>();

        public void Log(string message)
        {
            Messages.Add(message);
        }

        public IDisposable UsePrefix(string prefix)
        {
            return new Disposable(); // todo add action
        }
    }
}