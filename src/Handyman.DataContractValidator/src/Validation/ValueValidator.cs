using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.Validation
{
    internal class ValueValidator : TypeInfoValidator<ValueTypeInfo>
    {
        internal override void Validate(ValueTypeInfo actual, ValueTypeInfo expected, ValidationContext context)
        {
            var a = $"{actual.IsNullable ?? false} {actual.Value.FullName}";
            var e = $"{expected.IsNullable ?? false} {expected.Value.FullName}";

            if (a == e) return;

            context.AddError($"type mismatch, expected '{expected.TypeName}' but found '{actual.TypeName}'.");
        }
    }
}