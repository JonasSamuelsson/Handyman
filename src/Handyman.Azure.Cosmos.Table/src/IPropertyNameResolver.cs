using Azure.Data.Tables;
using System;
using System.Linq.Expressions;

namespace Handyman.Azure.Cosmos.Table
{
    public interface IPropertyNameResolver<TEntity>
        where TEntity : ITableEntity
    {
        void ETag();
        void PartitionKey();
        void RowKey();
        void Timestamp();
        void Property(string propertyName);
        void Property<TValue>(Expression<Func<TEntity, TValue>> propertyExpression);
    }
}