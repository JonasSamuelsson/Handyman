using Microsoft.Azure.Cosmos.Table;
using System.Linq;

namespace Handyman.Azure.Cosmos.Table
{
    public static class TableBatchResultExtensions
    {
        public static void EnsureSuccessStatusCode(this TableBatchResult batchResult)
        {
            batchResult.ForEach(x => x.EnsureSuccessStatusCode());
        }

        public static bool HasSuccessStatusCode(this TableBatchResult batchResult)
        {
            return batchResult.All(x => x.HasSuccessStatusCode());
        }
    }
}