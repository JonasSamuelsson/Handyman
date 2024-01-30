using Azure;

namespace Handyman.Azure.Cosmos.Table
{
    public static class ResponseExtensions
    {
        public static Response EnsureSuccessStatusCode(this Response response)
        {
            return response.HasSuccessStatusCode() ? response : throw new TableException(response.Status);
        }

        public static Response<T> EnsureSuccessStatusCode<T>(this Response<T> response)
        {
            return response.HasSuccessStatusCode() ? response : throw new TableException(response.GetRawResponse().Status);
        }

        public static bool HasSuccessStatusCode(this Response response)
        {
            return response.Status < 400;
        }

        public static bool HasSuccessStatusCode<T>(this Response<T> response)
        {
            return response.GetRawResponse().HasSuccessStatusCode();
        }

        public static TEntity GetEntityOrThrow<TEntity>(this Response<TEntity> response)
        {
            response.EnsureSuccessStatusCode();
            return response.Value ?? throw new TableException(500);
        }
    }
}