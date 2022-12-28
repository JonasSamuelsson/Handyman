using System;

namespace Handyman.Tools.Docs.Shared;

public interface IValueConverter
{
    public bool TryConvert(string s, Type targetType, out object value);
}