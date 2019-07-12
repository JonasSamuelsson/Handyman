using Microsoft.Azure.Cosmos.Table;

namespace Handyman.Azure.Cosmos.Table
{
    public class TableResult<TEntity>
        where TEntity : ITableEntity
    {
        private TEntity _entity;

        public string ETag { get; set; }
        public int HttpStatusCode { get; set; }

        public TEntity Entity
        {
            get => IsSuccessStatusCode ? _entity : throw new TableException(HttpStatusCode);
            set => _entity = value;
        }

        public bool IsSuccessStatusCode => HttpStatusCode < 400;

        public void AssertSuccessStatusCode()
        {
            if (IsSuccessStatusCode)
                return;

            throw new TableException(HttpStatusCode);
        }
    }
}