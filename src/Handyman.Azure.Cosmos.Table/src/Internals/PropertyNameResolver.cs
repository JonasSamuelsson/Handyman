using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Handyman.Azure.Cosmos.Table.Internals
{
    internal class PropertyNameResolver<TEntity> : IPropertyNameResolver<TEntity> where TEntity : ITableEntity
    {
        public List<string> Properties { get; } = new List<string>();

        public void ETag()
        {
            Property(nameof(ITableEntity.ETag));
        }

        public void PartitionKey()
        {
            Property(nameof(ITableEntity.PartitionKey));
        }

        public void RowKey()
        {
            Property(nameof(ITableEntity.RowKey));
        }

        public void Timestamp()
        {
            Property(nameof(ITableEntity.Timestamp));
        }

        public void Property(string propertyName)
        {
            Properties.Add(propertyName);
        }

        public void Property<TValue>(Expression<Func<TEntity, TValue>> propertyExpression)
        {
            Property(((MemberExpression)propertyExpression.Body).Member.Name);
        }
    }
}