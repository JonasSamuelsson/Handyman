using System;

namespace Handyman.AspNetCore.ETags.Internals;

internal class ETagConverter : IETagConverter
{
    public string FromByteArray(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        return ETagUtility.ToETag(bytes);
    }
}