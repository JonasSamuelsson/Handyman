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

            var hasIds = actual.HasIds && expected.HasIds;
            var hasNames = actual.HasNames && expected.HasNames;

            if (!hasIds)
            {
                a += $" [ {string.Join(", ", actual.Names.OrderBy(x => x))} ]";
                e += $" [ {string.Join(", ", expected.Names.OrderBy(x => x))} ]";
            }
            else if (!hasNames)
            {
                a += $" [ {string.Join(", ", actual.Ids.OrderBy(x => x))} ]";
                e += $" [ {string.Join(", ", expected.Ids.OrderBy(x => x))} ]";
            }
            else
            {
                a += $" [ {string.Join(", ", actual.Values.OrderBy(x => x.Id).Select(x => $"{x.Id}/{x.Name}"))} ]";
                e += $" [ {string.Join(", ", expected.Values.OrderBy(x => x.Id).Select(x => $"{x.Id}/{x.Name}"))} ]";
            }

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