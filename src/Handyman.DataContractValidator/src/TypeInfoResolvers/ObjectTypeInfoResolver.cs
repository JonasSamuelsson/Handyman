﻿using Handyman.DataContractValidator.Model;
using Handyman.DataContractValidator.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.DataContractValidator.TypeInfoResolvers
{
    internal class ObjectTypeInfoResolver : ITypeInfoResolver
    {
        private readonly ISet<object> _trace = new HashSet<object>();

        public bool TryResolveTypeInfo(object o, TypeInfoResolverContext context, out TypeInfo typeInfo)
        {
            var isType = o is Type;
            var type = o as Type ?? o.GetType();
            var isAnonymousType = type.Name.Contains("<>") && type.Name.Contains("AnonymousType");

            if (isAnonymousType && isType)
            {
                throw new NotSupportedException("Can't handle type of anonymous object.");
            }

            var marker = isAnonymousType ? (object)Guid.NewGuid() : type;

            var typeHasNullableAnnotations = NullableReferenceTypes.HasNullableAnnotations(type);

            var properties = new List<PropertyInfo>();

            foreach (var property in type.GetProperties())
            {
                if (!_trace.Add(marker))
                {
                    throw new InvalidOperationException($"Circular dependency, type: {type.FullName}.");
                }

                var isIgnored = property.GetCustomAttributes(true)
                    .Any(x => Regex.IsMatch(x.GetType().Name, "Ignore"));

                var child = isAnonymousType
                    ? property.GetValue(o, null)
                    : property.PropertyType;

                var childTypeInfo = context.GetTypeInfo(child);

                if (!childTypeInfo.IsNullable.HasValue && typeHasNullableAnnotations)
                {
                    childTypeInfo.IsNullable = NullableReferenceTypes.IsNullable(property);
                }

                properties.Add(new PropertyInfo
                {
                    IsIgnored = isIgnored,
                    Name = property.Name,
                    Type = childTypeInfo
                });

                _trace.Remove(marker);
            }

            typeInfo = new ObjectTypeInfo
            {
                Properties = properties
            };
            return true;
        }

        private static bool SupportsNullable(Type type)
        {
            return false;
        }

        private static bool IsNullable(System.Reflection.PropertyInfo property)
        {
            return false;
        }
    }
}