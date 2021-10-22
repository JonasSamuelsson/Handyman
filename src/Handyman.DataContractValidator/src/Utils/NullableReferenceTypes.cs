using System;
using System.Linq;
using System.Reflection;

namespace Handyman.DataContractValidator.Utils
{
    internal static class NullableReferenceTypes
    {
        // https://github.com/dotnet/roslyn/blob/main/docs/features/nullable-metadata.md

        private const string NullableAttributeName = "System.Runtime.CompilerServices.NullableAttribute";
        private const string NullableAttributeFieldName = "NullableFlags";
        private const string NullableContextAttributeName = "System.Runtime.CompilerServices.NullableContextAttribute";
        private const string NullableContextAttributeFieldName = "Flag";

        public static bool HasNullableAnnotations(Type type)
        {
            var properties = type.GetProperties();

            var hasReferenceTypeProperties = properties
                .Select(x => x.PropertyType)
                .Any(x => !x.IsValueType);

            if (!hasReferenceTypeProperties)
            {
                return true;
            }

            return type.GetCustomAttributes(true)
                .Any(x => x.GetType().FullName == NullableAttributeName);
        }

        public static bool IsNullable(PropertyInfo property)
        {
            var attribute = property.GetCustomAttributes(true)
                .SingleOrDefault(x => x.GetType().FullName == NullableAttributeName);

            if (attribute == null)
            {
                return IsNullableViaContextAttribute(property.DeclaringType);
            }

            var field = attribute.GetType().GetField(NullableAttributeFieldName);

            if (field == null)
            {
                throw new InvalidOperationException();
            }

            var bytes = (byte[])field.GetValue(attribute);

            return bytes.Length != 0 && bytes[0] == 2;
        }

        private static bool IsNullableViaContextAttribute(Type type)
        {
            var attribute = type.GetCustomAttributes(true)
                .SingleOrDefault(x => x.GetType().FullName == NullableContextAttributeName);

            if (attribute == null)
            {
                return false;
            }

            var field = attribute.GetType().GetField(NullableContextAttributeFieldName);

            if (field == null)
            {
                throw new InvalidOperationException();
            }

            var flag = (byte)field.GetValue(attribute);

            return flag == 2;
        }
    }
}