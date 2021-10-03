using Handyman.DataContractValidator.Model;
using System.Linq;

namespace Handyman.DataContractValidator.Validation
{
    internal class ObjectValidator : TypeInfoValidator<ObjectTypeInfo>
    {
        internal override void Validate(ObjectTypeInfo actual, ObjectTypeInfo expected, ValidationContext context)
        {
            var names = new[] { actual, expected }
                .SelectMany(x => x.Properties)
                .Select(x => x.Name)
                .Distinct()
                .OrderBy(x => x);

            foreach (var name in names)
            {
                var actualProperty = actual.Properties.FirstOrDefault(x => x.Name == name);
                var expectedProperty = expected.Properties.FirstOrDefault(x => x.Name == name);

                if (actualProperty == null)
                {
                    context.AddError(name, "expected property not found.");
                    continue;
                }

                if (expectedProperty == null)
                {
                    if (!actualProperty.IsIgnored)
                    {
                        context.AddError(name, "unexpected property.");
                    }

                    continue;
                }

                if (actualProperty.IsIgnored)
                {
                    context.AddError(name, "expected property found but it is decorated with ignore attribute.");
                    continue;
                }

                context.Validate(name, actualProperty.Type, expectedProperty.Type);
            }
        }
    }
}