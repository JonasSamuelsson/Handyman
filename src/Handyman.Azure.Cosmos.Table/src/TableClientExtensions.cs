using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Azure.Cosmos.Table;

public static class TableClientExtensions
{
    public static async Task<IReadOnlyList<T>> QueryAsync<T>(this TableClient client, TableQuery<T> query, CancellationToken cancellationToken) where T : class, ITableEntity
    {
        var filter = query.Filter;
        var top = int.MaxValue;
        var select = query.Select;

        await foreach (var page in client.QueryAsync<T>(filter, top, select, cancellationToken).AsPages().WithCancellation(cancellationToken))
        {
            return page.Values;
        }

        return Array.Empty<T>();
    }
}