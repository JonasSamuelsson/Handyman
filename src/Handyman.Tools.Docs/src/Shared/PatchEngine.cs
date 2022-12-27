using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Docs.Shared;

public static class PatchEngine
{
    public static IReadOnlyList<string> ApplyPatches(IEnumerable<string> lines, IEnumerable<Patch> patches)
    {
        var result = lines.ToList();

        foreach (var patch in patches.Reverse())
        {
            var element = patch.Element;
            var index = element.LineIndex;
            var name = $"handyman-docs:{element.Name}";
            var attributes = string.Join(" ", element.Attributes.Select(x => $"{x.Key}=\"{x.Value}\""));
            var nameAndAttributes = $"{name} {attributes}".Trim();

            result.RemoveRange(index, element.LineCount);

            result.Insert(index++, $"{element.Prefix}<{nameAndAttributes}>{element.Postfix}");
            result.InsertRange(index, patch.Content);
            result.Insert(index + patch.Content.Count, $"{element.Prefix}</{name}>{element.Postfix}");
        }

        return result;
    }

    public class Patch
    {
        public Element Element { get; set; }
        public IReadOnlyList<string> Content { get; set; }
    }
}