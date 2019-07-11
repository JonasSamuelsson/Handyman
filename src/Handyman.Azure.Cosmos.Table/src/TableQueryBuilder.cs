using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using Handyman.Azure.Cosmos.Table.Internals;

namespace Handyman.Azure.Cosmos.Table
{
    public class TableQueryBuilder : TableQueryBuilder<DynamicTableEntity> { }

    public class TableQueryBuilder<TEntity>
        where TEntity : ITableEntity
    {
        private readonly TableQuery<TEntity> _query = new TableQuery<TEntity>();

        public TableQueryBuilder<TEntity> Where(Action<ITableQueryFilterBuilder<TEntity>> action)
        {
            var builder = new TableQueryFilterBuilder<TEntity>();
            action.Invoke(builder);
            _query.Where(builder.Build());
            return this;
        }

        public TableQueryBuilder<TEntity> OrderBy(string propertyName)
        {
            _query.OrderBy(propertyName);
            return this;
        }

        public TableQueryBuilder<TEntity> OrderBy(Action<IPropertyNameResolver<TEntity>> action)
        {
            var resolver = new PropertyNameResolver<TEntity>();
            action.Invoke(resolver);
            return OrderBy(resolver.Properties.Single());
        }

        public TableQueryBuilder<TEntity> OrderByDesc(string propertyName)
        {
            _query.OrderByDesc(propertyName);
            return this;
        }

        public TableQueryBuilder<TEntity> OrderByDesc(Action<IPropertyNameResolver<TEntity>> action)
        {
            var resolver = new PropertyNameResolver<TEntity>();
            action.Invoke(resolver);
            return OrderByDesc(resolver.Properties.Single());
        }

        public TableQueryBuilder<TEntity> Select(IEnumerable<string> columns)
        {
            _query.Select(columns.ToList());
            return this;
        }

        public TableQueryBuilder<TEntity> Select(params Action<IPropertyNameResolver<TEntity>>[] actions)
        {
            var resolver = new PropertyNameResolver<TEntity>();

            foreach (var action in actions)
                action.Invoke(resolver);

            Select(resolver.Properties);

            return this;
        }

        public TableQueryBuilder<TEntity> Take(int? count)
        {
            _query.Take(count);
            return this;
        }

        public TableQuery<TEntity> Build()
        {
            return _query;
        }
    }
}