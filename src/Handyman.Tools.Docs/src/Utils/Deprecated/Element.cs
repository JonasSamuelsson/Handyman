using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils.Deprecated
{
    public class Element
    {
        public Attribute[] Attributes { get; set; }
        public int FromLineIndex { get; set; }
        public int LineCount { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }

        public int FromLineNumber => FromLineIndex + 1;
        public int? ContentLineIndex { get; set; }
        public int ContentLineCount { get; set; }

        public IEnumerable<string> Write(IEnumerable<string> content)
        {
            var attributes = string.Empty;

            foreach (var attribute in Attributes)
            {
                attributes += $" {attribute.Name}=\"{attribute.Value}\"";
            }

            yield return $"{Prefix}<handyman-docs:{Name}{attributes}>{Suffix}";

            foreach (var line in content)
            {
                yield return line;
            }

            yield return $"{Prefix}</handyman-docs:{Name}>{Suffix}";
        }
    }
}