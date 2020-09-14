using System;
using System.Text;

namespace Handyman.DuckTyping
{
    internal class CodeBuilder
    {
        private int _indentation = 0;
        private readonly StringBuilder _builder = new StringBuilder();

        public void Add(string s)
        {
            var prefix = new string(' ', _indentation);
            _builder.Append($"{prefix}{s}");
        }

        public void AddLine()
        {
            AddLine(string.Empty);
        }

        public void AddLine(string s)
        {
            Add($"{s}{Environment.NewLine}");
        }

        public IDisposable CreateScope()
        {
            AddLine("{");

            var delta = 2;
            _indentation += delta;

            return new Disposable
            {
                Action = () =>
                {
                    _indentation -= delta;
                    AddLine("}");
                }
            };
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        private class Disposable : IDisposable
        {
            public Action Action { get; set; }

            public void Dispose()
            {
                Action.Invoke();
            }
        }
    }
}