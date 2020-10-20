using System;

namespace Handyman.Tools.Docs.Utils
{
    public class Logger : ILogger
    {
        private readonly IConsoleWriter _consoleWriter;
        private string _prefix = string.Empty;

        public Logger() : this(new ConsoleWriter())
        {
        }

        public Logger(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public IDisposable CreateScope(string message)
        {
            WriteInfo($" > {message}");
            return UsePrefix("   ");
        }

        public IDisposable UsePrefix(string prefix)
        {
            var x = _prefix;
            _prefix += prefix;
            return new Disposable { Action = () => _prefix = x };
        }

        public void WriteError(string message)
        {
            _consoleWriter.WriteError($"{_prefix}{message}");
        }

        public void WriteInfo(string message)
        {
            _consoleWriter.WriteInfo($"{_prefix}{message}");
        }
    }
}