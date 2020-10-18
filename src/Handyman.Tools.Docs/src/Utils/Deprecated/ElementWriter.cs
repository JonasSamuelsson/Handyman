using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils.Deprecated
{
    public class ElementWriter : IElementWriter
    {
        public void Write(List<string> lines, Element element, IEnumerable<string> content)
        {
            lines.RemoveRange(element.FromLineIndex, element.LineCount);
            lines.InsertRange(element.FromLineIndex, element.Write(content));
        }
    }
}