using System;

namespace Handyman.Tools.Docs.Shared;

public interface IValueParser
{
    public bool TryParse(string s, Type targetType, out object value);
}