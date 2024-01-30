using Azure.Data.Tables;
using System.Collections.Generic;

namespace Handyman.Azure.Cosmos.Table;

public class TableQuery<TEntity> where TEntity : ITableEntity
{
    internal string Filter { get; set; }
    internal IEnumerable<string> Select { get; set; }
}