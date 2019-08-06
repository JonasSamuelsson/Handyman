using Microsoft.Azure.Cosmos.Table;

namespace Handyman.Azure.Cosmos.Table
{
    public static class CloudTableClientExtensions
    {
        public static T GetTableReference<T>(this CloudTableClient client, string tableName)
            where T : CloudTable
        {
            return (T)CloudTableFactory.CreateCloudTable(typeof(T), client, tableName);
        }
    }
}
