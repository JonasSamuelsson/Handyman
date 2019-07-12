using Microsoft.Azure.Cosmos.Table;

namespace Handyman.Azure.Cosmos.Table
{
    public static class TableResultExtensions
    {
        public static void AssertSuccessStatusCode(this TableResult result)
        {
            if (result.IsSuccessStatusCode())
                return;

            throw new TableException(result.HttpStatusCode);
        }

        public static bool IsSuccessStatusCode(this TableResult result)
        {
            return result.HttpStatusCode < 400;
        }
    }
}