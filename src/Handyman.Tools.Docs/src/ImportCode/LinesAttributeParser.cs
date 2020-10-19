using Handyman.Tools.Docs.Utils.Deprecated;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Docs.ImportCode
{
    public static class LinesAttributeParser
    {
        private static readonly Regex Regex = new Regex("^(?<from>\\d+)(-(?<to>\\d+))?$");

        public static bool Validate(string lines, List<string> errors)
        {
            var isValid = true;
            var match = Regex.Match(lines);

            if (match.Success)
            {
                var toGroup = match.Groups["to"];

                if (toGroup.Success)
                {
                    var fromGroup = match.Groups["from"];
                    var from = int.Parse(fromGroup.Value);
                    var to = int.Parse(toGroup.Value);

                    if (to < from)
                    {
                        isValid = false;
                        errors.Add("'lines': from can't be greater than to.");
                    }
                }
            }
            else
            {
                isValid = false;
                errors.Add("'lines' has an invalid format.");
            }

            return isValid;
        }

        public static LinesAttribute Deserialize(string value)
        {
            throw new NetworkInformationException();
            //var match = Regex.Match(value);

            //var fromNumber = int.Parse(match.Groups["from"].Value);
            //var toGroup = match.Groups["to"];
            //var toIndex = toGroup.Success ? int.Parse(toGroup.Value) : fromNumber;

            //return new LinesAttribute
            //{
            //    FromLineNumber = fromNumber,
            //    LineCount = toIndex - fromNumber
            //};
        }
    }

    public class ReferenceElementAttributeParser : IAttributesValidator, IAttributesDeserializer<ReferenceElementAttributes>
    {
        public bool CanHandle(string elementName)
        {
            return elementName == "region";
        }

        public bool Validate(IReadOnlyCollection<Attribute> attributes, List<string> errors)
        {
            var dictionary = attributes.ToDictionary(x => x.Name, x => x.Value);

            return dictionary.TryGetValue("id", out var id) && !string.IsNullOrWhiteSpace(id);
        }

        public ReferenceElementAttributes Deserialize(IReadOnlyCollection<Attribute> attributes)
        {
            return new ReferenceElementAttributes
            {
                Id = attributes.Single(x => x.Name == "id").Value
            };
        }
    }

    public class ReferenceElementAttributes
    {
        public string Id { get; set; }
    }
}