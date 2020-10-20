using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IDataSerializer<TData>
        where TData : ElementData
    {
        bool TryDeserialize(IReadOnlyList<KeyValuePair<string, string>> keyValuePairs, ILogger logger, out TData data);
        string Serialize(TData data);
    }

    public abstract class ElementData
    {
        public IReadOnlyCollection<string> AttributeOrder { get; set; }
    }
}