using System;

namespace Handyman.Azure.Cosmos.Table
{
    public interface ITableQueryFilterConditionBuilder
    {
        void Equals(bool value);
        void Equals(byte[] value);
        void Equals(DateTimeOffset value);
        void Equals(double value);
        void Equals(Guid value);
        void Equals(int value);
        void Equals(long value);
        void Equals(string value);
        void GreaterThan(bool value);
        void GreaterThan(byte[] value);
        void GreaterThan(DateTimeOffset value);
        void GreaterThan(double value);
        void GreaterThan(Guid value);
        void GreaterThan(int value);
        void GreaterThan(long value);
        void GreaterThan(string value);
        void GreaterThanOrEquals(bool value);
        void GreaterThanOrEquals(byte[] value);
        void GreaterThanOrEquals(DateTimeOffset value);
        void GreaterThanOrEquals(double value);
        void GreaterThanOrEquals(Guid value);
        void GreaterThanOrEquals(int value);
        void GreaterThanOrEquals(long value);
        void GreaterThanOrEquals(string value);
        void LessThan(bool value);
        void LessThan(byte[] value);
        void LessThan(DateTimeOffset value);
        void LessThan(double value);
        void LessThan(Guid value);
        void LessThan(int value);
        void LessThan(long value);
        void LessThan(string value);
        void LessThanOrEquals(bool value);
        void LessThanOrEquals(byte[] value);
        void LessThanOrEquals(DateTimeOffset value);
        void LessThanOrEquals(double value);
        void LessThanOrEquals(Guid value);
        void LessThanOrEquals(int value);
        void LessThanOrEquals(long value);
        void LessThanOrEquals(string value);
        void NotEquals(bool value);
        void NotEquals(byte[] value);
        void NotEquals(DateTimeOffset value);
        void NotEquals(double value);
        void NotEquals(Guid value);
        void NotEquals(int value);
        void NotEquals(long value);
        void NotEquals(string value);
        void StartsWith(string value);
    }

    public interface ITableQueryFilterConditionBuilder<TValue>
    {
        void Equals(TValue value);
        void GreaterThan(TValue value);
        void GreaterThanOrEquals(TValue value);
        void LessThan(TValue value);
        void LessThanOrEquals(TValue value);
        void NotEquals(TValue value);
    }
}