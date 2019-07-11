using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq.Expressions;

namespace Handyman.Azure.Cosmos.Table
{
    public interface ITableQueryFilterBuilder<TEntity>
        where TEntity : ITableEntity
    {
        ITableQueryFilterConditionBuilder<string> ETag { get; }
        ITableQueryFilterConditionBuilder<string> PartitionKey { get; }
        ITableQueryFilterConditionBuilder<string> RowKey { get; }
        ITableQueryFilterConditionBuilder<DateTimeOffset> Timestamp { get; }

        void And(params Action<ITableQueryFilterBuilder<TEntity>>[] actions);
        void Or(params Action<ITableQueryFilterBuilder<TEntity>>[] actions);
        void Not(Action<ITableQueryFilterBuilder<TEntity>> action);

        ITableQueryFilterConditionBuilder Property(string name);
        ITableQueryFilterConditionBuilder<bool> Property(Expression<Func<TEntity, bool>> property);
        ITableQueryFilterConditionBuilder<byte[]> Property(Expression<Func<TEntity, byte[]>> property);
        ITableQueryFilterConditionBuilder<DateTimeOffset> Property(Expression<Func<TEntity, DateTimeOffset>> property);
        ITableQueryFilterConditionBuilder<double> Property(Expression<Func<TEntity, double>> property);
        ITableQueryFilterConditionBuilder<Guid> Property(Expression<Func<TEntity, Guid>> property);
        ITableQueryFilterConditionBuilder<int> Property(Expression<Func<TEntity, int>> property);
        ITableQueryFilterConditionBuilder<long> Property(Expression<Func<TEntity, long>> property);
        ITableQueryFilterConditionBuilder<string> Property(Expression<Func<TEntity, string>> property);
    }
}