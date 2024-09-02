using System;

namespace Handyman.AspNetCore.ETags.Internals;

internal class ETagComparer : IETagComparer
{
    public void EnsureEquals(string eTag, byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(eTag);
        ArgumentNullException.ThrowIfNull(bytes);

        ETagUtility.EnsureEquals(eTag, bytes);
    }

    public void EnsureEquals(string eTag1, string eTag2)
    {
        ArgumentNullException.ThrowIfNull(eTag1);
        ArgumentNullException.ThrowIfNull(eTag2);

        ETagUtility.EnsureEquals(eTag1, eTag2);
    }

    public bool Equals(string eTag, byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(eTag);
        ArgumentNullException.ThrowIfNull(bytes);

        return ETagUtility.Equals(eTag, bytes);
    }

    public bool Equals(string eTag1, string eTag2)
    {
        ArgumentNullException.ThrowIfNull(eTag1);
        ArgumentNullException.ThrowIfNull(eTag2);

        return ETagUtility.Equals(eTag1, eTag2);
    }
}