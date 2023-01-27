using System;
using System.Collections;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.Shared;

public class Attributes : IEnumerable<KeyValuePair<string, string>>
{
    private readonly Dictionary<string, string> _dictionary = new(StringComparer.OrdinalIgnoreCase);

    public void Add(string name, string value)
    {
        _dictionary.Add(name, value);
    }

    public bool Contains(string name)
    {
        return TryGet(name, out _);
    }

    public string Get(string name)
    {
        return TryGet(name, out var value) ? value : throw new TodoException();
    }

    public bool TryGet(string name, out string value)
    {
        return _dictionary.TryGetValue(name, out value);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}