#nullable enable

using Handyman.AspNetCore.ETags.Internals;
using System;
using System.Linq;

namespace Handyman.AspNetCore.ETags;

public static class ETagUtility
{
    public static string ToETag(byte[] bytes)
    {
        return $"W/\"{ToETagValue(bytes)}\"";
    }

    public static string ToETag(string s)
    {
        if (s == "*")
        {
            return s;
        }

        var value = GetETagValueOrThrow(s);

        if (value.Length != s.Length)
        {
            return s;
        }

        return $"W/\"{value}\"";
    }

    public static string ToETagValue(byte[] bytes)
    {
        var strings = bytes
            .SkipWhile(x => x == 0)
            .Select(x => x.ToString("x2"));

        var eTagValue = string.Join("", strings);

        return eTagValue.Length != 0
            ? eTagValue
            : "0";
    }

    public static string ToETagValue(string s)
    {
        var value = GetETagValueOrThrow(s);

        if (value.Length == s.Length)
        {
            return s;
        }

        return value.ToString();
    }

    public static bool Equals(string eTag1, byte[] eTag2)
    {
        if (eTag1 == "*")
        {
            return true;
        }

        return Equals(eTag1, ToETagValue(eTag2));
    }

    public static bool Equals(string eTag1, string eTag2)
    {
        if (eTag1 == "*" || eTag2 == "*")
        {
            return true;
        }

        var value1 = GetETagValueOrThrow(eTag1);
        var value2 = GetETagValueOrThrow(eTag2);

        return value1.SequenceEqual(value2);
    }

    public static void EnsureEquals(string eTag1, byte[] eTag2)
    {
        if (Equals(eTag1, eTag2))
        {
            return;
        }

        throw new PreconditionFailedException();
    }

    public static void EnsureEquals(string eTag1, string eTag2)
    {
        if (Equals(eTag1, eTag2))
        {
            return;
        }

        throw new PreconditionFailedException();
    }

    internal static ReadOnlySpan<char> GetETagValueOrThrow(string eTag)
    {
        var startsWith = eTag.StartsWith("W/\"");
        var endsWith = eTag.EndsWith("\"");

        var span = eTag.AsSpan();

        if (startsWith && endsWith)
        {
            span = span.Slice(3, span.Length - 4);
        }
        else if (!startsWith && !endsWith)
        {
            // nothing to do here
        }
        else
        {
            ThrowInvalidETagException();
        }

        if (span.Length == 0)
        {
            ThrowInvalidETagException();
        }

        return span;
    }

    internal static bool IsValidETag(string candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return false;
        }

        if (candidate == "*")
        {
            return true;
        }

        if (candidate.Length <= 4)
        {
            return false;
        }

        if (!candidate.StartsWith("W/\""))
        {
            return false;
        }

        if (!candidate.EndsWith("\""))
        {
            return false;
        }

        if (candidate.AsSpan(3, candidate.Length - 4).Contains('"'))
        {
            return false;
        }

        return true;
    }

    public static void ThrowInvalidETagException()
    {
        throw new ArgumentException("Invalid eTag format/value.");
    }
}