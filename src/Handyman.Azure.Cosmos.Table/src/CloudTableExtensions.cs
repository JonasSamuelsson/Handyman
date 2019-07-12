using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Azure.Cosmos.Table
{
    public static class CloudTableExtensions
    {
        public static Task<TableResult<TEntity>> DeleteEntityAsync<TEntity>(this CloudTable table, TEntity entity)
            where TEntity : ITableEntity
        {
            return DeleteEntityAsync(table, entity, CancellationToken.None);
        }

        public static Task<TableResult<TEntity>> DeleteEntityAsync<TEntity>(this CloudTable table, TEntity entity, CancellationToken cancellationToken)
            where TEntity : ITableEntity
        {
            var operation = TableOperation.Delete(entity);
            return ExecuteAsync<TEntity>(table, operation, cancellationToken);
        }

        public static Task<TableResult<TEntity>> InsertEntityAsync<TEntity>(this CloudTable table, TEntity entity)
            where TEntity : ITableEntity
        {
            return InsertEntityAsync(table, entity, CancellationToken.None);
        }

        public static Task<TableResult<TEntity>> InsertEntityAsync<TEntity>(this CloudTable table, TEntity entity, CancellationToken cancellationToken)
            where TEntity : ITableEntity
        {
            var operation = TableOperation.Insert(entity);
            return ExecuteAsync<TEntity>(table, operation, cancellationToken);
        }

        public static Task<TableResult<TEntity>> InsertOrMergeEntityAsync<TEntity>(this CloudTable table, TEntity entity)
            where TEntity : ITableEntity
        {
            return InsertOrMergeEntityAsync(table, entity, CancellationToken.None);
        }

        public static Task<TableResult<TEntity>> InsertOrMergeEntityAsync<TEntity>(this CloudTable table, TEntity entity, CancellationToken cancellationToken)
            where TEntity : ITableEntity
        {
            var operation = TableOperation.InsertOrMerge(entity);
            return ExecuteAsync<TEntity>(table, operation, cancellationToken);
        }

        public static Task<TableResult<TEntity>> InsertOrReplaceEntityAsync<TEntity>(this CloudTable table, TEntity entity)
            where TEntity : ITableEntity
        {
            return InsertOrReplaceEntityAsync(table, entity, CancellationToken.None);
        }

        public static Task<TableResult<TEntity>> InsertOrReplaceEntityAsync<TEntity>(this CloudTable table, TEntity entity, CancellationToken cancellationToken)
            where TEntity : ITableEntity
        {
            var operation = TableOperation.InsertOrReplace(entity);
            return ExecuteAsync<TEntity>(table, operation, cancellationToken);
        }

        public static Task<TableResult<TEntity>> MergeEntityAsync<TEntity>(this CloudTable table, TEntity entity)
            where TEntity : ITableEntity
        {
            return MergeEntityAsync(table, entity, CancellationToken.None);
        }

        public static Task<TableResult<TEntity>> MergeEntityAsync<TEntity>(this CloudTable table, TEntity entity, CancellationToken cancellationToken)
            where TEntity : ITableEntity
        {
            var operation = TableOperation.Merge(entity);
            return ExecuteAsync<TEntity>(table, operation, cancellationToken);
        }

        public static Task<List<TEntity>> QueryEntitiesAsync<TEntity>(this CloudTable table, TableQuery<TEntity> query)
            where TEntity : ITableEntity, new()
        {
            return table.QueryEntitiesAsync(query, CancellationToken.None);
        }

        public static async Task<List<TEntity>> QueryEntitiesAsync<TEntity>(this CloudTable table, TableQuery<TEntity> query, CancellationToken cancellationToken)
            where TEntity : ITableEntity, new()
        {
            var entities = new List<TEntity>();
            TableContinuationToken continuationToken = null;

            do
            {
                var segment = await table.ExecuteQuerySegmentedAsync(query, continuationToken, cancellationToken);
                entities.AddRange(segment);
                continuationToken = segment.ContinuationToken;
            } while (continuationToken != null);

            return entities;
        }

        public static Task<TableResult<TEntity>> ReplaceEntityAsync<TEntity>(this CloudTable table, TEntity entity)
            where TEntity : ITableEntity
        {
            return ReplaceEntityAsync(table, entity, CancellationToken.None);
        }

        public static Task<TableResult<TEntity>> ReplaceEntityAsync<TEntity>(this CloudTable table, TEntity entity, CancellationToken cancellationToken)
            where TEntity : ITableEntity
        {
            var operation = TableOperation.Replace(entity);
            return ExecuteAsync<TEntity>(table, operation, cancellationToken);
        }

        public static Task<TableResult<TEntity>> RetrieveEntityAsync<TEntity>(this CloudTable table, string partitionKey, string rowKey)
            where TEntity : ITableEntity
        {
            return RetrieveEntityAsync<TEntity>(table, partitionKey, rowKey, CancellationToken.None);
        }

        public static Task<TableResult<TEntity>> RetrieveEntityAsync<TEntity>(this CloudTable table, string partitionKey, string rowKey, CancellationToken cancellationToken)
            where TEntity : ITableEntity
        {
            var operation = TableOperation.Retrieve<TEntity>(partitionKey, rowKey);
            return ExecuteAsync<TEntity>(table, operation, cancellationToken);
        }

        private static async Task<TableResult<TEntity>> ExecuteAsync<TEntity>(CloudTable table, TableOperation operation, CancellationToken cancellationToken)
            where TEntity : ITableEntity
        {
            var result = await table.ExecuteAsync(operation, cancellationToken).ConfigureAwait(false);

            return new TableResult<TEntity>
            {
                ETag = result.Etag,
                HttpStatusCode = result.HttpStatusCode,
                Entity = (TEntity)result.Result
            };
        }
    }
}
