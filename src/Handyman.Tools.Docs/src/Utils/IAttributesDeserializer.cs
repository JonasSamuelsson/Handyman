using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IAttributesDeserializer<T>
    {
        T Deserialize(IReadOnlyCollection<Attribute> attributes);
    }
}