using Microsoft.Azure.Cosmos.Table;

namespace Handyman.Azure.Cosmos.Table
{
    public class TableResult<TEntity>
        where TEntity : ITableEntity
    {
        public TEntity Entity { get; set; }
        public string ETag { get; set; }
        public int HttpStatusCode { get; set; }
        public double? RequestCharge { get; set; }
        public string SessionToken { get; set; }

        public bool HasSuccessStatusCode => HttpStatusCode < 400;

        public void EnsureSuccessStatusCode()
        {
            if (HasSuccessStatusCode)
                return;

            throw new TableException(HttpStatusCode);
        }

        public TEntity GetEntityOrThrow()
        {
            EnsureSuccessStatusCode();
            return Entity;
        }

        public bool TryGetEntity(out TEntity entity)
        {
            if (HasSuccessStatusCode)
            {
                entity = Entity;
                return true;
            }

            entity = default;
            return false;
        }
    }
}