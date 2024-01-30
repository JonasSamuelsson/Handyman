using Azure.Data.Tables;
using Handyman.Azure.Cosmos.Table.Internals;
using System;
using System.Collections.Generic;

namespace Handyman.Azure.Cosmos.Table
{
    public class TableQueryBuilder : TableQueryBuilder<TableEntity>
    {
    }

    public class TableQueryBuilder<TEntity>
        where TEntity : ITableEntity
    {
        private readonly TableQuery<TEntity> _query = new TableQuery<TEntity>();

        public TableQueryBuilder<TEntity> Where(Action<ITableQueryFilterBuilder<TEntity>> action)
        {
            var builder = new TableQueryFilterBuilder<TEntity>();
            action.Invoke(builder);
            return Where(builder.Build());
        }

        public TableQueryBuilder<TEntity> Where(string filter)
        {
            if (_query.Filter != null)
            {
                throw new InvalidOperationException("Can't call Where more than once.");
            }

            _query.Filter = filter;
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

        public TableQueryBuilder<TEntity> Select(IEnumerable<string> properties)
        {
            if (_query.Select != null)
            {
                throw new InvalidOperationException("Can't call Select more than once.");
            }

            _query.Select = properties;
            return this;
        }

        public TableQuery<TEntity> Build()
        {
            return _query;
        }
    }
}