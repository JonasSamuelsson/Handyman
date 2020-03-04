using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;

namespace Handyman.Azure.Cosmos.Table
{
    public class TableQueryResult<TEntity>
        where TEntity : ITableEntity
    {
        public List<TEntity> Entities { get; } = new List<TEntity>();
        public double RequestCharge { get; set; }
    }
}