using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Enums
{
    internal class EnumHandler : IHandler
    {
        public bool TryGetTypeName(object o, ValidationContext context, out string name)
        {
            if (o is Enum)
            {
                name = "enum";
                return true;
            }

            if (o is Type type && type.IsEnum)
            {
                name = "enum";
                return true;
            }

            name = null;
            return false;
        }

        public bool TryGetValidator(object dataContract, out IValidator validator)
        {
            if (dataContract is Enum @enum)
            {
                validator = new EnumValidator(@enum);
                return true;
            }

            if (dataContract is Type type)
            {
                var isNullable = false;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    isNullable = true;
                    type = type.GetGenericArguments().Single();
                }

                if (type.IsEnum)
                {
                    var values = System.Enum.GetValues(type).Cast<long>();
                    var kvps = values.Select(i => new KeyValuePair<long, string>(i, System.Enum.GetName(type, i)));
                    var isFlags = type.GetCustomAttributes(typeof(FlagsAttribute), false).Any();

                    validator = new EnumValidator(new Enum(kvps)
                    {
                        Flags = isFlags,
                        Nullable = isNullable
                    });

                    return true;
                }
            }

            validator = null;
            return false;
        }
    }
}