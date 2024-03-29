﻿using Azure.Data.Tables;
using System;
using System.Linq.Expressions;

namespace Handyman.Azure.Cosmos.Table.Internals
{
    public class TableQueryFilterBuilder<TEntity> : ITableQueryFilterBuilder<TEntity>
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

        public ITableQueryFilterStringConditionBuilder ETag => Property(e => e.ETag.ToString());
        public ITableQueryFilterStringConditionBuilder PartitionKey => Property(e => e.PartitionKey);
        public ITableQueryFilterStringConditionBuilder RowKey => Property(e => e.RowKey);
        public ITableQueryFilterConditionBuilder<DateTimeOffset> Timestamp => Property(e => e.Timestamp.Value);

        public void And(Action<ITableQueryFilterBuilder<TEntity>> actions)
        {
            Combine(new AndTableQueryFilterNode(), actions);
        }

        public void Or(Action<ITableQueryFilterBuilder<TEntity>> actions)
        {
            Combine(new OrTableQueryFilterNode(), actions);
        }

        private void Combine(CompositionTableQueryFilterNode node, Action<ITableQueryFilterBuilder<TEntity>> actions)
        {
            _node.Add(node);
            var builder = new TableQueryFilterBuilder<TEntity>(node);
            actions.Invoke(builder);
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

        public ITableQueryFilterConditionBuilder<byte[]> Property(Expression<Func<TEntity, byte[]>> property)
        {
            return Property(property, FilterConditionGenerator.GenerateFilterConditionForBinary);
        }

        public ITableQueryFilterConditionBuilder<bool> Property(Expression<Func<TEntity, bool>> property)
        {
            return Property(property, FilterConditionGenerator.GenerateFilterConditionForBool);
        }

        public ITableQueryFilterConditionBuilder<DateTimeOffset> Property(Expression<Func<TEntity, DateTimeOffset>> property)
        {
            return Property(property, FilterConditionGenerator.GenerateFilterConditionForDate);
        }

        public ITableQueryFilterConditionBuilder<double> Property(Expression<Func<TEntity, double>> property)
        {
            return Property(property, FilterConditionGenerator.GenerateFilterConditionForDouble);
        }

        public ITableQueryFilterConditionBuilder<Guid> Property(Expression<Func<TEntity, Guid>> property)
        {
            return Property(property, FilterConditionGenerator.GenerateFilterConditionForGuid);
        }

        public ITableQueryFilterConditionBuilder<int> Property(Expression<Func<TEntity, int>> property)
        {
            return Property(property, FilterConditionGenerator.GenerateFilterConditionForInt);
        }

        public ITableQueryFilterConditionBuilder<long> Property(Expression<Func<TEntity, long>> property)
        {
            return Property(property, FilterConditionGenerator.GenerateFilterConditionForLong);
        }

        public ITableQueryFilterStringConditionBuilder Property(Expression<Func<TEntity, string>> property)
        {
            var propertyName = GetPropertyName(property);
            return new TableQueryFilterStringConditionBuilder(propertyName, FilterConditionGenerator.GenerateFilterConditionForString, _node);
        }

        private ITableQueryFilterConditionBuilder<TValue> Property<TValue>(Expression<Func<TEntity, TValue>> property, FilterConditionGeneratorDelegate<TValue> generator)
        {
            var propertyName = GetPropertyName(property);
            return new TableQueryFilterConditionBuilder<TValue>(propertyName, generator, _node);
        }

        private static string GetPropertyName<TValue>(Expression<Func<TEntity, TValue>> property)
        {
            return ((MemberExpression)property.Body).Member.Name;
        }

        public string Build()
        {
            return _node.Build();
        }
    }
}