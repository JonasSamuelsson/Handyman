using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

            var dataContractIsType = _dataContract is Type;

            var dataContractType = dataContractIsType
                ? (Type)_dataContract
                : _dataContract.GetType();

            var recursiveTypesFeature = context.Features.GetOrAdd<RecursiveTypesFeature>();
            var scope = $"{type.FullName}::{dataContractType.FullName}";

            if (!recursiveTypesFeature.TryAdd(scope))
                return;

            var expectedProperties = dataContractIsType
                ? dataContractType.GetProperties().ToDictionary(x => x.Name, x => (object)x.PropertyType)
                : dataContractType.GetProperties().ToDictionary(x => x.Name, x => x.GetValue(_dataContract, null));

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
                        .Any(x => Regex.IsMatch(x.GetType().Name, "Ignore[a-zA-Z0-9]"));

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

                    context.Validate(actualProperty.PropertyType, expectedProperty);
                }
            }

            recursiveTypesFeature.Remove(scope);
        }

        private class RecursiveTypesFeature
        {
            private readonly HashSet<string> _set = new HashSet<string>();

            public bool TryAdd(string scope) => _set.Add(scope);

            public void Remove(string scope) => _set.Remove(scope);
        }
    }
}