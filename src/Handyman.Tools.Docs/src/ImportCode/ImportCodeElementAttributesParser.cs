using Handyman.Tools.Docs.Utils.Deprecated;
using System;
using System.Collections.Generic;
using System.Linq;
using Attribute = Handyman.Tools.Docs.Utils.Deprecated.Attribute;

namespace Handyman.Tools.Docs.ImportCode
{
    public class ImportCodeElementAttributesParser : IAttributesValidator, IAttributesDeserializer<ImportCodeElementAttributes>
    {
        public bool CanHandle(string elementName)
        {
            return elementName == "import-code";
        }

        public bool Validate(IReadOnlyCollection<Attribute> attributes, List<string> errors)
        {
            var isValid = true;
            var dictionary = attributes.ToDictionary(x => x.Name, x => x.Value);

            if (!dictionary.TryGetValue("src", out var path))
            {
                isValid = false;
                errors.Add("'src' is required.");
            }

            var hasId = dictionary.TryGetValue("id", out var id);
            var hasLines = dictionary.TryGetValue("lines", out var lines);

            if (hasId && hasLines)
            {
                isValid = false;
                errors.Add("'id' and 'lines' can't be combined.");
            }

            if (hasId && string.IsNullOrWhiteSpace(id))
            {
                isValid = false;
                errors.Add("'id' can't be empty.");
            }

            if (hasLines && LinesAttributeParser.Validate(lines, errors) == false)
            {
                isValid = false;
            }

            var validAttributes = new[] { "id", "lines", "src", "language" };

            foreach (var attribute in attributes)
            {
                if (validAttributes.Contains(attribute.Name))
                    continue;

                isValid = false;
                errors.Add($"'{attribute.Name}' is not supported.");
            }

            return isValid;
        }

        public ImportCodeElementAttributes Deserialize(IReadOnlyCollection<Attribute> attributes)
        {
            var dictionary = attributes.ToDictionary(x => x.Name, x => x.Value);

            var result = new ImportCodeElementAttributes();

            if (dictionary.TryGetValue("id", out var id))
            {
                result.Id = id;
            }

            if (dictionary.TryGetValue("language", out var language))
            {
                result.Language = language;
            }

            if (dictionary.TryGetValue("lines", out var lines))
            {
                result.Lines = LinesAttributeParser.Deserialize(lines);
            }

            if (dictionary.TryGetValue("src", out var src))
            {
                result.Source = src;
            }

            return result;
        }
    }
}