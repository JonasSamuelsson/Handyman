using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Docs.Shared;

public class LinesSpec
{
    public IReadOnlyList<Section> Sections { get; set; }

    public IReadOnlyList<string> ReadFrom(IReadOnlyList<string> lines)
    {
        var result = new List<string>();

        foreach (var section in Sections)
        {
            if (section.FromIndex < 0)
            {
                throw new TodoException();
            }

            if (section.FromIndex + section.Count > lines.Count)
            {
                throw new TodoException();
            }

            result.AddRange(lines.Skip(section.FromIndex).Take(section.Count));
        }

        return result;
    }

    public static LinesSpec CreateForAllOf(IReadOnlyList<string> lines)
    {
        return CreateForSection(1, lines.Count);
    }

    public static LinesSpec CreateForSection(int fromNumber, int count)
    {
        return new LinesSpec
        {
            Sections = new[]
            {
                new Section
                {
                    Count = count,
                    FromNumber = fromNumber
                }
            }
        };
    }

    public class Section
    {
        public int Count { get; set; }
        public int FromNumber { get; set; }

        public int FromIndex => FromNumber - 1;
        public int ToNumber => FromNumber + Count - 1;

        public override string ToString()
        {
            return Count == 1 ? $"{FromNumber}" : $"{FromNumber}-{ToNumber}";
        }
    }
}