﻿namespace Handyman.AspNetCore.ETags.Internals;

internal class ETagUtilities : IETagUtilities
{
    public ETagUtilities(IETagComparer comparer, IETagConverter converter)
    {
        Comparer = comparer;
        Converter = converter;
    }

    public IETagComparer Comparer { get; }
    public IETagConverter Converter { get; }
}