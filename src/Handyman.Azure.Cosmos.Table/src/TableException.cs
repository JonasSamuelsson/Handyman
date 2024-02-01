using System;

namespace Handyman.Azure.Cosmos.Table
{
    public class TableException : Exception
    {
        public TableException(int statusCode)
        {
            StatusCode = statusCode;
        }

        public TableException(int statusCode, Exception exception) : base(null, exception)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; }
    }
}