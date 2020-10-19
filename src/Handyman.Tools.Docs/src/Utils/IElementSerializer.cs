using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IElementSerializer<TData>
    {
        IEnumerable<Element<TData>> TryDeserializeElements(string elementName, IReadOnlyList<string> lines, ILogger logger);
        void WriteElement(Element<TData> element, IReadOnlyCollection<string> content, List<string> lines);
    }
}