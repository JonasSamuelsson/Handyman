using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Docs.Utils
{
    public class Logger : ILogger
    {
        private readonly IConsoleWriter _consoleWriter;
        private readonly Collection _scopes = new Collection();
        private readonly Collection _prefixes = new Collection();

        public Logger() : this(new ConsoleWriter())
        {
        }

        public Logger(IConsoleWriter consoleWriter)
        {
            _consoleWriter = consoleWriter;
        }

        public IDisposable CreateScope(string scope)
        {
            var prefix = GetPrefix();
            var item = _scopes.Add($"{prefix}{scope}");
            return new Disposable
            {
                Action = () => _scopes.Remove(item),
                InnerDisposable = UsePrefix("  ")
            };
        }

        public IDisposable UsePrefix(string prefix)
        {
            var item = _prefixes.Add(prefix);
            return new Disposable { Action = () => _prefixes.Remove(item) };
        }

        public void WriteError(string message)
        {
            WriteScopes();
            _consoleWriter.WriteError($"{GetPrefix()}{message}");
        }

        public void WriteInfo(string message)
        {
            WriteScopes();
            _consoleWriter.WriteInfo($"{GetPrefix()}{message}");
        }

        private void WriteScopes()
        {
            foreach (var scope in _scopes.Items)
            {
                _consoleWriter.WriteInfo(scope.Text);
                _scopes.Remove(scope);
            }
        }

        private string GetPrefix()
        {
            return string.Join("", _prefixes.Items.Select(x => x.Text));
        }

        private class Collection
        {
            private readonly List<Item> _list = new List<Item>();

            public IEnumerable<Item> Items => _list.ToList();

            public Item Add(string s)
            {
                var item = new Item { Text = s };
                _list.Add(item);
                return item;
            }

            public void Remove(Item item)
            {
                _list.Remove(item);
            }
        }

        private class Item
        {
            public string Text { get; set; }
        }
    }
}