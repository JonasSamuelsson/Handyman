using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Enums
{
    internal class EnumValidator : IValidator
    {
        private readonly Enum _enum;

        public EnumValidator(Enum @enum)
        {
            _enum = @enum;
        }

        public string GetTypeName(ValidationContext context)
        {
            return GetExpectedTypeString();
        }

        public void Validate(Type type, ValidationContext context)
        {
            var actualType = GetActualTypeString(type, context, out var enumType);
            var expectedType = GetExpectedTypeString();

            if (actualType != expectedType)
            {
                context.AddError($"type mismatch, expected {expectedType} but found {actualType}");
                return;
            }

            var actualValues = System.Enum.GetValues(enumType)
                .Cast<object>()
                .ToDictionary(x => (long)Convert.ChangeType(x, typeof(long)), x => System.Enum.GetName(enumType, x));

            var expectedCount = _enum.Values.Count;

            if (actualValues.Count != expectedCount)
            {
                AddError(context, actualValues);
                return;
            }

            if (!_enum.HasIds)
            {
                if (_enum.Names.Intersect(actualValues.Values, StringComparer.OrdinalIgnoreCase).Count() != expectedCount)
                {
                    AddError(context, actualValues);
                }

                return;
            }

            if (!_enum.HasNames)
            {
                if (_enum.Ids.Intersect(actualValues.Keys).Count() != expectedCount)
                {
                    AddError(context, actualValues);
                }

                return;
            }

            var actual = actualValues.Select(x => $"{x.Key}/{x.Value}");
            var expected = _enum.Values.Select(x => $"{x.Id}/{x.Name}");

            if (actual.Intersect(expected, StringComparer.OrdinalIgnoreCase).Count() != expectedCount)
            {
                AddError(context, actualValues);
            }
        }

        private string GetActualTypeString(Type type, ValidationContext context, out Type enumType)
        {
            enumType = type;
            var strings = new List<string>();

            if (enumType.IsGenericType && enumType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                enumType = type.GetGenericArguments().Single();
                strings.Add("nullable");
            }

            if (!enumType.IsEnum)
                return context.GetTypeName(type);

            if (enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
                strings.Add("flags");

            strings.Add("enum");

            return string.Join(" ", strings);
        }

        private string GetExpectedTypeString()
        {
            var strings = new List<string>();

            if (_enum.Nullable)
                strings.Add("nullable");

            if (_enum.Flags)
                strings.Add("flags");

            strings.Add("enum");

            return string.Join(" ", strings);
        }

        private void AddError(ValidationContext context, Dictionary<long, string> actualValues)
        {
            var actual = "";
            var expected = "";

            if (!_enum.HasIds)
            {
                actual = string.Join(",", actualValues.Values);
                expected = string.Join(",", _enum.Names);
            }
            else if (!_enum.HasNames)
            {
                actual = string.Join(",", actualValues.Keys);
                expected = string.Join(",", _enum.Ids);
            }
            else
            {
                actual = string.Join(",", actualValues.Select(x => $"{x.Key}/{x.Value}"));
                expected = string.Join(",", _enum.Values.Select(x => $"{x.Id}/{x.Name}"));
            }

            context.Errors.Add($"enum values mismatch, expected [{expected}] but found [{actual}].");
        }
    }
}