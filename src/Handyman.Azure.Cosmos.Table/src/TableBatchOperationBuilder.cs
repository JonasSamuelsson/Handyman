using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Azure.Cosmos.Table
{
    public class TableBatchOperationBuilder
    {
        private readonly List<TableOperation> _operations = new List<TableOperation>();

        public TableBatchOperationBuilder Delete(ITableEntity entity) => Add(TableOperation.Delete(entity));
        public TableBatchOperationBuilder Insert(ITableEntity entity) => Add(TableOperation.Insert(entity));
        public TableBatchOperationBuilder InsertOrMerge(ITableEntity entity) => Add(TableOperation.InsertOrMerge(entity));
        public TableBatchOperationBuilder InsertOrReplace(ITableEntity entity) => Add(TableOperation.InsertOrReplace(entity));
        public TableBatchOperationBuilder Merge(ITableEntity entity) => Add(TableOperation.Merge(entity));
        public TableBatchOperationBuilder Replace(ITableEntity entity) => Add(TableOperation.Replace(entity));

        public IEnumerable<TableBatchOperation> Build()
        {
            return _operations
                .Select((operation, i) => new { operation, batch = i / 100 })
                .GroupBy(x => x.batch, x => x.operation)
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var batch = new TableBatchOperation();
                    foreach (var operation in g) batch.Add(operation);
                    return batch;
                });
        }

        private TableBatchOperationBuilder Add(TableOperation operation)
        {
            _operations.Add(operation);
            return this;
        }
    }
}