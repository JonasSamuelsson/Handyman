using Microsoft.Azure.Cosmos.Table;
using System;
using System.Globalization;
using System.Reflection;

namespace Handyman.Azure.Cosmos.Table
{
    public static class CloudTableClientExtensions
    {
        public static T GetTableReference<T>(this CloudTableClient client, string tableName)
            where T : CloudTable
        {
            var type = typeof(T);
            var flags = BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.NonPublic;
            var args = new object[] { tableName, client };

            return (T)Activator.CreateInstance(type, flags, (Binder)null, args, (CultureInfo)null);
        }
    }
}