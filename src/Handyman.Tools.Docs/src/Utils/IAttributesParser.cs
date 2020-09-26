using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IAttributesParser
    {
        bool CanHandle(string elementName);
        bool Validate(IReadOnlyCollection<Attribute> attributes);
    }

    public interface IAttributesParser<T>
    {
        T Deserialize(IReadOnlyCollection<Attribute> attributes);
    }
}