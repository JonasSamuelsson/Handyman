using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IAttributesValidator
    {
        bool CanHandle(string elementName);
        bool Validate(IReadOnlyCollection<Attribute> attributes, List<string> errors);
    }
}