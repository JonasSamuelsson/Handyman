using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Handyman.Azure.Cosmos.Table
{
    public static class TableQueryFilterBuilder
    {
        public static ITableQueryFilterBuilder<TEntity> For<TEntity>() where TEntity : ITableEntity
        {
            return new TableQueryFilterBuilder<TEntity>();
        }
    }

    internal class TableQueryFilterBuilder<TEntity> : ITableQueryFilterBuilder<TEntity>
        where TEntity : ITableEntity
    {
        private readonly ITableQueryFilterNode _node;

        public TableQueryFilterBuilder()
            : this(new TableQueryFilterRootNode())
        {
        }

        private TableQueryFilterBuilder(ITableQueryFilterNode node)
        {
            _node = node;
        }

        public ITableQueryFilterConditionBuilder<string> ETag => Property(e => e.ETag);
        public ITableQueryFilterConditionBuilder<string> PartitionKey => Property(e => e.PartitionKey);
        public ITableQueryFilterConditionBuilder<string> RowKey => Property(e => e.RowKey);
        public ITableQueryFilterConditionBuilder<DateTimeOffset> Timestamp => Property(e => e.Timestamp);

        public void And(params Action<ITableQueryFilterBuilder<TEntity>>[] actions)
        {
            Combine(TableOperators.And, actions);
        }

        public void Or(params Action<ITableQueryFilterBuilder<TEntity>>[] actions)
        {
            Combine(TableOperators.Or, actions);
        }

        private void Combine(string operation, IEnumerable<Action<ITableQueryFilterBuilder<TEntity>>> actions)
        {
            var node = new TableQueryFilterCompositionNode(operation);
            _node.Add(node);
            var builder = new TableQueryFilterBuilder<TEntity>(node);

            foreach (var action in actions)
                action.Invoke(builder);
        }

        public void Not(Action<ITableQueryFilterBuilder<TEntity>> action)
        {
            var node = new TableQueryFilterNegateNode();
            _node.Add(node);
            var builder = new TableQueryFilterBuilder<TEntity>(node);
            action.Invoke(builder);
        }

        public ITableQueryFilterConditionBuilder Property(string name)
        {
            return new TableQueryFilterConditionBuilder(name, _node);
        }

        public ITableQueryFilterConditionBuilder<bool> Property(Expression<Func<TEntity, bool>> property)
        {
            return Property(property, TableQuery.GenerateFilterConditionForBool);
        }

        public ITableQueryFilterConditionBuilder<byte[]> Property(Expression<Func<TEntity, byte[]>> property)
        {
            return Property(property, TableQuery.GenerateFilterConditionForBinary);
        }

        public ITableQueryFilterConditionBuilder<DateTimeOffset> Property(Expression<Func<TEntity, DateTimeOffset>> property)
        {
            return Property(property, TableQuery.GenerateFilterConditionForDate);
        }

        public ITableQueryFilterConditionBuilder<double> Property(Expression<Func<TEntity, double>> property)
        {
            return Property(property, TableQuery.GenerateFilterConditionForDouble);
        }

        public ITableQueryFilterConditionBuilder<Guid> Property(Expression<Func<TEntity, Guid>> property)
        {
            return Property(property, TableQuery.GenerateFilterConditionForGuid);
        }

        public ITableQueryFilterConditionBuilder<int> Property(Expression<Func<TEntity, int>> property)
        {
            return Property(property, TableQuery.GenerateFilterConditionForInt);
        }

        public ITableQueryFilterConditionBuilder<long> Property(Expression<Func<TEntity, long>> property)
        {
            return Property(property, TableQuery.GenerateFilterConditionForLong);
        }

        public ITableQueryFilterConditionBuilder<string> Property(Expression<Func<TEntity, string>> property)
        {
            return Property(property, TableQuery.GenerateFilterCondition);
        }

        private ITableQueryFilterConditionBuilder<TValue> Property<TValue>(Expression<Func<TEntity, TValue>> property, FilterConditionGenerator<TValue> generator)
        {
            var propertyName = ((MemberExpression)property.Body).Member.Name;
            return new TableQueryFilterConditionBuilder<TValue>(propertyName, generator, _node);
        }

        internal string Build()
        {
            return _node.Build();
        }
    }
}