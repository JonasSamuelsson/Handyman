using System.Collections.Generic;

namespace Handyman.Tools.Docs.Shared;

public interface IElementReader
{
    IReadOnlyList<Element> ReadElements(string name, IReadOnlyList<string> lines);
}