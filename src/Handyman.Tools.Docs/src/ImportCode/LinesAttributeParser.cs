using System.Collections.Generic;
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
            var match = Regex.Match(value);

            var fromIndex = int.Parse(match.Groups["from"].Value) - 1;
            var toGroup = match.Groups["to"];
            var toIndex = toGroup.Success ? int.Parse(toGroup.Value) : fromIndex + 1;

            return new LinesAttribute
            {
                FromLineIndex = fromIndex,
                LineCount = toIndex - fromIndex
            };
        }
    }
}