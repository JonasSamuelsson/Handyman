using System;
using System.Globalization;
using System.Text;

namespace Handyman.Azure.Cosmos.Table.Internals;

internal class FilterConditionGenerator
{
    public static string GenerateFilterConditionForBinary(string property, string operation, byte[] value)
    {
        var builder = new StringBuilder();

        foreach (var b in value)
        {
            builder.Append($"{b:x2}");
        }

        var s = builder.ToString();

        return GenerateFilterCondition(property, operation, $"X'{s}'");
    }

    public static string GenerateFilterConditionForBool(string property, string operation, bool value)
    {
        var s = value ? "true" : "false";

        return GenerateFilterCondition(property, operation, s);
    }

    public static string GenerateFilterConditionForDate(string property, string operation, DateTimeOffset value)
    {
        var s = value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffffff");

        s = $"datetime'{s}Z'";

        return GenerateFilterCondition(property, operation, s);
    }

    public static string GenerateFilterConditionForDouble(string property, string operation, double value)
    {
        var s = Convert.ToString(value, CultureInfo.InvariantCulture);

        if (int.TryParse(s, out _))
        {
            s = $"{s}.0";
        }

        return GenerateFilterCondition(property, operation, s);
    }

    public static string GenerateFilterConditionForGuid(string property, string operation, Guid value)
    {
        var s = $"guid'{value}'";

        return GenerateFilterCondition(property, operation, s);
    }

    public static string GenerateFilterConditionForInt(string property, string operation, int value)
    {
        var s = value.ToString(CultureInfo.InvariantCulture);

        return GenerateFilterCondition(property, operation, s);
    }

    public static string GenerateFilterConditionForLong(string property, string operation, long value)
    {
        var s = $"{value.ToString(CultureInfo.InvariantCulture)}L";

        return GenerateFilterCondition(property, operation, s);
    }

    public static string GenerateFilterConditionForString(string property, string operation, string value)
    {
        var s = $"'{value.Replace("'", "''")}'";

        return GenerateFilterCondition(property, operation, s);
    }

    private static string GenerateFilterCondition(string property, string operation, string value)
    {
        return $"{property} {operation} {value}";
    }

    public static string CombineFilters(string filterA, string @operator, string filterB)
    {
        return $"({filterA}) {@operator} ({filterB})";
    }
}