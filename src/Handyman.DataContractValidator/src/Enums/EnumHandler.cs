using System;
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
                validator = new EnumValidator(@enum.EnumKind, @enum.EnumValues);
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
                    var isFlags = type.GetCustomAttributes(typeof(FlagsAttribute), false).Any();

                    var enumKind = EnumKind.Default
                                   | (isFlags ? EnumKind.Flags : EnumKind.Default)
                                   | (isNullable ? EnumKind.Nullable : EnumKind.Default);
                    var enumValues = System.Enum.GetValues(type).Cast<long>();
                    validator = new EnumValidator(enumKind, enumValues);
                    return true;
                }
            }

            validator = null;
            return false;
        }
    }
}