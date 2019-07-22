using Microsoft.Azure.Cosmos.Table;
using System;

namespace Handyman.Azure.Cosmos.Table
{
    public static class CloudTableClientExtensions
    {
        public static T GetTableReference<T>(this CloudTableClient client, string tableName)
            where T : CloudTable
        {
            return (T)Activator.CreateInstance(typeof(T), tableName, client);
        }
    }
}