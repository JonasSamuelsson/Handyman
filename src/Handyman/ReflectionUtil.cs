using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Handyman
{
    public static class ReflectionUtil
    {
        public static string GetPropertyName(Expression<Func<object>> expression)
        {
            return GetPropertyInfo(expression).Name;
        }

        private static PropertyInfo GetPropertyInfo(Expression expression)
        {
            var lambda = expression as LambdaExpression;
            if (lambda == null) throw new NotSupportedException();
            var member = lambda.Body as MemberExpression;
            if (member == null) throw new NotSupportedException();
            var property = member.Member as PropertyInfo;
            if (property == null) throw new NotSupportedException();
            return property;
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            return GetPropertyInfo(expression).Name;
        }

        public static string GetPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<string> GetPropertyNames(Expression<Func<object>> expression)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<string> GetPropertyNames<T>(Expression<Func<T, object>> expression)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<string> GetPropertyNames<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            throw new NotImplementedException();
        }

        public static object GetPropertyValue(object instance, string property)
        {
            throw new NotImplementedException();
        }

        public static T GetPropertyValue<T>(object instance, string property)
        {
            return (T)GetPropertyValue(instance, property);
        }

        public static object GetPropertyValue(object instance, IEnumerable<string> properties)
        {
            throw new NotImplementedException();
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
    }
}