using Handyman.DataContractValidator.Model;
using System.Linq;

namespace Handyman.DataContractValidator.Validation
{
    internal class EnumValidator : TypeInfoValidator<EnumTypeInfo>
    {
        internal override void Validate(EnumTypeInfo actual, EnumTypeInfo expected, ValidationContext context)
        {
            var a = GetCoreTypeInfo(actual);
            var e = GetCoreTypeInfo(expected);

            a += $" [ {string.Join(", ", actual.Values.OrderBy(x => x.Key).Select(x => $"{x.Key}/{x.Value}"))} ]";
            e += $" [ {string.Join(", ", expected.Values.OrderBy(x => x.Key).Select(x => $"{x.Key}/{x.Value}"))} ]";

            if (a == e)
            {
                return;
            }

            context.AddError($"type mismatch, expected '{e}' but found '{a}'.");
        }

        private static string GetCoreTypeInfo(EnumTypeInfo typeInfo)
        {
            var segments = new[]
            {
                typeInfo.IsNullable == true ? "nullable" : "",
                typeInfo.IsFlags ? "flags":"",
                "enum"
            };

            return string.Join(" ", segments.Where(x => !string.IsNullOrEmpty(x)));
        }
    }
}