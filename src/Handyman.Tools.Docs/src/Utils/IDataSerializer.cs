using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IDataSerializer<TData>
    {
        bool TryDeserialize(IReadOnlyList<KeyValuePair<string, string>> keyValuePairs, ILogger logger, out TData data);
        string Serialize(TData data);
    }
}