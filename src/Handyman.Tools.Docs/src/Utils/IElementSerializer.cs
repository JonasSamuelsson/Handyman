using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IElementSerializer<TData>
    {
        bool TryDeserializeElements(string elementName, IReadOnlyList<string> lines, ILogger logger, out IReadOnlyList<Element<TData>> elements);
        void WriteElement(Element<TData> element, IReadOnlyCollection<string> content, List<string> lines);
    }
}