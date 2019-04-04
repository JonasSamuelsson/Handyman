using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Enums
{
    internal class EnumValidator : IValidator
    {
        private readonly EnumKind _enumKind;
        private readonly IEnumerable<string> _expectedValues;

        public EnumValidator(EnumKind enumKind, IEnumerable<long> values)
        {
            _enumKind = enumKind;
            _expectedValues = values.Select(x => x.ToString()).ToList();
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

            var underlyingType = System.Enum.GetUnderlyingType(enumType);
            var actualValues = System.Enum.GetValues(enumType).Cast<object>().Select(x => Convert.ChangeType(x, underlyingType).ToString()).ToList();

            if (actualValues.Except(_expectedValues).Concat(_expectedValues.Except(actualValues)).Any())
            {
                var expectedValuesString = string.Join(",", _expectedValues);
                var actualValuesString = string.Join(",", actualValues);
                context.AddError($"enum values mismatch, expected [{expectedValuesString}] but found [{actualValuesString}]");
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

            if (_enumKind.HasFlag(EnumKind.Nullable))
                strings.Add("nullable");

            if (_enumKind.HasFlag(EnumKind.Flags))
                strings.Add("flags");

            strings.Add("enum");

            return string.Join(" ", strings);
        }
    }
}