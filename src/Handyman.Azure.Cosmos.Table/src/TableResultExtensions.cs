using Microsoft.Azure.Cosmos.Table;

namespace Handyman.Azure.Cosmos.Table
{
    public static class TableResultExtensions
    {
        public static void EnsureSuccessStatusCode(this TableResult result)
        {
            if (result.HasSuccessStatusCode())
                return;

            throw new TableException(result.HttpStatusCode);
        }

        public static bool HasSuccessStatusCode(this TableResult result)
        {
            return result.HttpStatusCode < 400;
        }

        public static TEntity GetEntityOrThrow<TEntity>(this TableResult result)
        {
            result.EnsureSuccessStatusCode();
            return (TEntity)result.Result;
        }

        public static bool TryGetEntity<TEntity>(this TableResult result, out TEntity entity)
        {
            if (result.HasSuccessStatusCode())
            {
                entity = (TEntity)result.Result;
                return true;
            }

            entity = default;
            return false;
        }
    }
}