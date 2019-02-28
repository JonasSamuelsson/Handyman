using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Enums
{
    internal class EnumValidator : IValidator
    {
        private readonly bool _expectedIsFlagsEnum;
        private readonly IEnumerable<string> _expectedValues;

        public EnumValidator(bool isFlagsEnum, IEnumerable<long> values)
        {
            _expectedIsFlagsEnum = isFlagsEnum;
            _expectedValues = values.Select(x => x.ToString()).ToList();
        }

        public string GetTypeName(ValidationContext context)
        {
            return "enum";
        }

        public void Validate(Type type, ValidationContext context)
        {
            if (!type.IsEnum)
            {
                var typeName = context.GetTypeName(type);
                context.AddError($"type mismatch, expected enum but found {typeName}");
                return;
            }

            var actualIsFlagsEnum = type.GetCustomAttributes(typeof(FlagsAttribute), false).Any();

            if (actualIsFlagsEnum != _expectedIsFlagsEnum)
            {
                var expectedKind = _expectedIsFlagsEnum ? "flags" : "values";
                var actualKind = actualIsFlagsEnum ? "flags" : "values";

                context.AddError($"enum kind mismatch, expected {expectedKind} but found {actualKind}");
            }

            var underlyingType = System.Enum.GetUnderlyingType(type);
            var actualValues = System.Enum.GetValues(type).Cast<object>().Select(x => Convert.ChangeType(x, underlyingType).ToString()).ToList();

            if (actualValues.Except(_expectedValues).Concat(_expectedValues.Except(actualValues)).Any())
            {
                var expectedValuesString = string.Join(",", _expectedValues);
                var actualValuesString = string.Join(",", actualValues);
                context.AddError($"enum values mismatch, expected [{expectedValuesString}] but found [{actualValuesString}]");
            }
        }
    }
}