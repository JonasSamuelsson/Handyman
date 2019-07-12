using System;

namespace Handyman.Azure.Cosmos.Table
{
    public class TableException : Exception
    {
        public TableException(int httpStatusCode)
        {
            HttpStatusCode = httpStatusCode;
        }

        public int HttpStatusCode { get; }
    }
}