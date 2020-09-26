using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IElementWriter
    {
        void Write(List<string> lines, Element element, IEnumerable<string> content);
    }
}