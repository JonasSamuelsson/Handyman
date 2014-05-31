using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Handyman
{
    public static class ReflectionUtil
    {
        public static string GetPropertyName(Expression<Func<object>> expression)
        {
            return GetPropertyNames(expression).Single();
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            return GetPropertyNames(expression).Single();
        }

        public static IReadOnlyList<string> GetPropertyNames(Expression<Func<object>> expression)
        {
            return GetPropertyInfos(expression)
                .Select(x => x.Name)
                .ToList();
        }

        public static IReadOnlyList<string> GetPropertyNames<T>(Expression<Func<T, object>> expression)
        {
            return GetPropertyInfos(expression)
                .Select(x => x.Name)
                .ToList();
        }

        public static object GetPropertyValue(object instance, string property, params string[] properties)
        {
            return GetPropertyValue(instance, properties.Prepend(property));
        }

        public static T GetPropertyValue<T>(object instance, string property, params string[] properties)
        {
            return (T)GetPropertyValue(instance, properties.Prepend(property));
        }

        public static object GetPropertyValue(object instance, IEnumerable<string> properties)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (properties == null) throw new ArgumentNullException("properties");
            properties = properties.ToList();
            if (properties.IsEmpty()) throw new ArgumentException();
            foreach (var property in properties)
            {
                var type = instance.GetType();
                var propertyInfo = type.GetProperty(property);
                instance = propertyInfo.GetValue(instance, null);
            }
            return instance;
        }

        public static T GetPropertyValue<T>(object instance, IEnumerable<string> properties)
        {
            return (T)GetPropertyValue(instance, properties);
        }

        public static void SetPropertyValue(object instance, string property, object targetValue)
        {
            throw new NotImplementedException();
        }

        public static void SetPropertyValue(object instance, IEnumerable<string> properties, object targetValue)
        {
            throw new NotImplementedException();
        }

        private static IReadOnlyList<PropertyInfo> GetPropertyInfos(Expression expression)
        {
            var lambda = expression as LambdaExpression;
            if (lambda == null) throw new NotSupportedException();
            var member = (lambda.Body is UnaryExpression
                              ? ((UnaryExpression)lambda.Body).Operand
                              : lambda.Body) as MemberExpression;
            if (member == null) throw new NotSupportedException();
            var properties = new List<PropertyInfo>();
            for (; member != null; member = member.Expression as MemberExpression)
                properties.Insert(0, (PropertyInfo)member.Member);
            return properties;
        }
    }
}