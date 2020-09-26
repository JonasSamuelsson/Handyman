using Handyman.Tools.Docs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Attribute = Handyman.Tools.Docs.Utils.Attribute;

namespace Handyman.Tools.Docs.ImportCode
{
    public class ImportCodeElementAttributesParser : IAttributesParser, IAttributesParser<ImportCodeElementAttributes>
    {
        public bool CanHandle(string elementName)
        {
            return elementName == "import-code";
        }

        public bool Validate(IReadOnlyCollection<Attribute> attributes)
        {
            var dictionary = attributes.ToDictionary(x => x.Name, x => x.Value);

            if (!dictionary.TryGetValue("path", out var path))
                throw new NotImplementedException();

            return true;
        }

        public ImportCodeElementAttributes Deserialize(IReadOnlyCollection<Attribute> attributes)
        {
            throw new NotImplementedException();
        }
    }
}