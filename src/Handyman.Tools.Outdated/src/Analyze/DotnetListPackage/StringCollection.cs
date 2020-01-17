using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    [DebuggerDisplay("{Current}")]
    public class StringCollection
    {
        private readonly List<string> _lines;
        private int _index;

        public StringCollection(IEnumerable<string> lines)
        {
            _lines = lines.Select(x => x.Trim()).ToList();
        }

        public string Current => _lines.ElementAtOrDefault(_index) ?? string.Empty;
        public bool Finished => Current == string.Empty;

        public void MoveNext()
        {
            _index++;
        }
    }
}