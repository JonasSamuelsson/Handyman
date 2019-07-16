using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Handyman.Azure.Cosmos.Table.Internals
{
    internal class TableQueryFilterBuilder<TEntity> : ITableQueryFilterBuilder<TEntity>
        where TEntity : ITableEntity
    {
        private readonly ITableQueryFilterNode _node;

        public TableQueryFilterBuilder()
            : this(new RootTableQueryFilterNode())
        {
        }

        private TableQueryFilterBuilder(ITableQueryFilterNode node)
        {
            _node = node;
        }

        public ITableQueryFilterStringConditionBuilder ETag => Property(e => e.ETag);
        public ITableQueryFilterStringConditionBuilder PartitionKey => Property(e => e.PartitionKey);
        public ITableQueryFilterStringConditionBuilder RowKey => Property(e => e.RowKey);
        public ITableQueryFilterConditionBuilder<DateTimeOffset> Timestamp => Property(e => e.Timestamp);

        public void And(params Action<ITableQueryFilterBuilder<TEntity>>[] actions)
        {
            Combine(new AndTableQueryFilterNode(), actions);
        }

        public void Or(params Action<ITableQueryFilterBuilder<TEntity>>[] actions)
        {
            Combine(new OrTableQueryFilterNode(), actions);
        }

        private void Combine(CompositionTableQueryFilterNode node, IEnumerable<Action<ITableQueryFilterBuilder<TEntity>>> actions)
        {
            _node.Add(node);
            var builder = new TableQueryFilterBuilder<TEntity>(node);

            foreach (var action in actions)
                action.Invoke(builder);
        }

        public void Not(Action<ITableQueryFilterBuilder<TEntity>> action)
        {
            var node = new NegateTableQueryFilterNode();
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

        public ITableQueryFilterStringConditionBuilder Property(Expression<Func<TEntity, string>> property)
        {
            var propertyName = GetPropertyName(property);
            return new TableQueryFilterStringConditionBuilder(propertyName, TableQuery.GenerateFilterCondition, _node);
        }

        private ITableQueryFilterConditionBuilder<TValue> Property<TValue>(Expression<Func<TEntity, TValue>> property, FilterConditionGenerator<TValue> generator)
        {
            var propertyName = GetPropertyName(property);
            return new TableQueryFilterConditionBuilder<TValue>(propertyName, generator, _node);
        }

        private static string GetPropertyName<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            return ((MemberExpression)property.Body).Member.Name;
        }

        internal string Build()
        {
            return _node.Build();
        }
    }
}