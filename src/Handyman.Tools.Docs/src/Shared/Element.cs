using System.Collections.Generic;

namespace Handyman.Tools.Docs.Shared
{
    public class Element
    {
        public IReadOnlyList<string> Content { get; set; }
        public int LineIndex { get; set; }
        public int LineCount { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public Attributes Attributes { get; set; }
        public string Postfix { get; set; }

        public int ContentLineCount => LineCount - 2;
        public int ContentLineNumber => LineIndex + 2;
        public int LineNumber => LineIndex + 1;
    }
}