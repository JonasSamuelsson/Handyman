using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public abstract class ElementData
    {
        public IReadOnlyCollection<string> AttributeOrder { get; set; }
    }
}