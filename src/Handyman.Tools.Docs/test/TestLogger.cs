using Handyman.Tools.Docs.Utils;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.Tests
{
    internal class TestLogger : Logger
    {
        private readonly TestConsoleWriter _consoleWriter;

        public TestLogger() : this(new TestConsoleWriter())
        {
        }

        private TestLogger(TestConsoleWriter consoleWriter) : base(consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public IReadOnlyList<string> Messages => _consoleWriter.Messages;

        private class TestConsoleWriter : IConsoleWriter
        {
            public List<string> Messages { get; } = new List<string>();

            public void WriteError(string message)
            {
                Messages.Add($"e:{message}");
            }

            public void WriteInfo(string message)
            {
                Messages.Add($"i:{message}");
            }
        }
    }
}