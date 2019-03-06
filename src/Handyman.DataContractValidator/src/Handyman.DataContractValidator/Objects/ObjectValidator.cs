using System;
using System.Collections;
using System.Linq;

namespace Handyman.DataContractValidator.Objects
{
    internal class ObjectValidator : IValidator
    {
        private readonly object _dataContract;

        public ObjectValidator(object dataContract)
        {
            _dataContract = dataContract;
        }

        public string GetTypeName(ValidationContext context)
        {
            return "object";
        }

        public void Validate(Type type, ValidationContext context)
        {
            var actualProperties = type.GetProperties();

            var expectedProperties = _dataContract is Type dataContractType
                ? dataContractType.GetProperties().ToDictionary(x => x.Name, x => (object)x.PropertyType)
                : _dataContract.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(_dataContract, null));

            var names = actualProperties
                .Select(x => x.Name)
                .Concat(expectedProperties.Keys)
                .OrderBy(x => x)
                .Distinct();

            foreach (var name in names)
            {
                using (context.CreateScope(name))
                {
                    var actualProperty = actualProperties.FirstOrDefault(x => x.Name == name);

                    if (actualProperty == null)
                    {
                        context.AddError("expected property not found");
                        continue;
                    }

                    var actualPropertyIsIgnored = actualProperty.GetCustomAttributes(false)
                        .Any(x => x.GetType().Name == "JsonIgnoreAttribute");

                    if (!expectedProperties.TryGetValue(name, out var expectedProperty))
                    {
                        if (actualPropertyIsIgnored)
                            continue;

                        context.AddError("unexpected property");
                        continue;
                    }

                    if (actualPropertyIsIgnored)
                    {
                        context.AddError("expected property found but decorated with ignore attribute");
                        continue;
                    }

                    if (expectedProperty.GetType().Namespace != null && actualProperty.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(actualProperty.PropertyType))
                    {
                        var actualType = actualProperty.PropertyType.IsArray
                            ? actualProperty.PropertyType.GetElementType()
                            : actualProperty.PropertyType.GetGenericArguments()[0];

                        string expected;
                        switch (expectedProperty)
                        {
                            case Type[] types:
                                expected = types[0].FullName;
                                break;
                            case Type expectedType:
                                expected = expectedType.GetGenericArguments()[0].FullName;
                                break;
                            default:
                                expected = expectedProperty.GetType().GetGenericArguments()[0].FullName;
                                break;
                        }

                        if (actualType != null && actualType.FullName == expected)
                            continue;

                    }

                    if (actualProperty.PropertyType.FullName == expectedProperty.ToString())
                        continue;

                    context.Validate(actualProperty.PropertyType, expectedProperty);
                }
            }
        }
    }
}