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

        public bool IsInRange(IReadOnlyCollection<string> lines, ILogger logger)
        {
            // todo validate, needs nullable count & to properties
            return true;
        }

        public List<string> Apply(IReadOnlyCollection<string> lines)
        {
            return lines
                .Skip(FromIndex)
                .Take(Count)
                .ToList();
        }
    }
}