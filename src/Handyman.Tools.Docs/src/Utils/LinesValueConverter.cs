using System.Linq;

namespace Handyman.Tools.Docs.Utils
{
    public class LinesValueConverter : ValueConverter<Lines>
    {
        public override bool TryConvertFromString(string s, ILogger logger, out Lines value)
        {
            value = null;

            if (int.TryParse(s, out var from))
            {
                if (from < 1)
                {
                    logger.WriteError($"Invalid format '{s}', value can't be less than 1.");
                    return false;
                }

                value = new Lines
                {
                    Count = 1,
                    FromNumber = from,
                    Text = s
                };

                return true;
            }

            if (TryParse(s, '-', out from, out var to))
            {
                if (from < 1)
                {
                    logger.WriteError($"Invalid format '{s}', from can't be less than 1.");
                    return false;
                }

                if (to < @from)
                {
                    logger.WriteError($"Invalid format '{s}', from can't greater than to.");
                    return false;
                }

                value = new Lines
                {
                    Count = (to - from) + 1,
                    FromNumber = from,
                    Text = s
                };

                return true;
            }

            if (TryParse(s, '+', out from, out var count))
            {
                if (from < 1)
                {
                    logger.WriteError($"Invalid format '{s}', from can't be less than 1.");
                    return false;
                }

                if (count < 1)
                {
                    logger.WriteError($"Invalid format '{s}', count can't be less than 1.");
                    return false;
                }

                value = new Lines
                {
                    Count = count + 1,
                    FromNumber = from,
                    Text = s
                };

                return true;
            }

            logger.WriteError($"Invalid format '{s}'.");
            return false;
        }

        private static bool TryParse(string s, char separator, out int first, out int second)
        {
            first = 0;
            second = 0;

            if (s.Count(x => x == separator) != 1)
                return false;

            var index = s.IndexOf(separator);

            if (index <= 0 || index == s.Length - 1)
                return false;

            var s1 = s.Substring(0, index);
            var s2 = s.Substring(index + 1);

            return int.TryParse(s1, out first) && int.TryParse(s2, out second);
        }

        public override string ConvertToString(Lines value)
        {
            return value.Text;
        }
    }
}