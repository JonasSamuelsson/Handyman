using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Docs.Utils
{
    public class Lines
    {
        public int FromNumber { get; set; }
        public int Count { get; set; }
        public string Text { get; set; }

        public int FromIndex => FromNumber - 1;

        public bool TryTrim(List<string> lines)
        {
            if (FromIndex + Count >= lines.Count)
                return false;

            var temp = lines
                .Skip(FromIndex)
                .Take(Count)
                .ToList();

            lines.Clear();
            lines.AddRange(temp);

            return true;
        }
    }
}