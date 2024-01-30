using Azure;
using Azure.Data.Tables;
using System;

namespace Handyman.Azure.Cosmos.Table;

public class TableEntityBase : ITableEntity
{
    public ETag ETag { set; get; }
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}