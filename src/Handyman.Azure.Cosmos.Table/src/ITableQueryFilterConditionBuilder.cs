using System;

namespace Handyman.Azure.Cosmos.Table
{
    public interface ITableQueryFilterConditionBuilder
    {
        void Equal(bool value);
        void Equal(byte[] value);
        void Equal(DateTimeOffset value);
        void Equal(double value);
        void Equal(Guid value);
        void Equal(int value);
        void Equal(long value);
        void Equal(string value);
        void GreaterThan(bool value);
        void GreaterThan(byte[] value);
        void GreaterThan(DateTimeOffset value);
        void GreaterThan(double value);
        void GreaterThan(Guid value);
        void GreaterThan(int value);
        void GreaterThan(long value);
        void GreaterThan(string value);
        void GreaterThanOrEqual(bool value);
        void GreaterThanOrEqual(byte[] value);
        void GreaterThanOrEqual(DateTimeOffset value);
        void GreaterThanOrEqual(double value);
        void GreaterThanOrEqual(Guid value);
        void GreaterThanOrEqual(int value);
        void GreaterThanOrEqual(long value);
        void GreaterThanOrEqual(string value);
        void LessThan(bool value);
        void LessThan(byte[] value);
        void LessThan(DateTimeOffset value);
        void LessThan(double value);
        void LessThan(Guid value);
        void LessThan(int value);
        void LessThan(long value);
        void LessThan(string value);
        void LessThanOrEqual(bool value);
        void LessThanOrEqual(byte[] value);
        void LessThanOrEqual(DateTimeOffset value);
        void LessThanOrEqual(double value);
        void LessThanOrEqual(Guid value);
        void LessThanOrEqual(int value);
        void LessThanOrEqual(long value);
        void LessThanOrEqual(string value);
        void NotEqual(bool value);
        void NotEqual(byte[] value);
        void NotEqual(DateTimeOffset value);
        void NotEqual(double value);
        void NotEqual(Guid value);
        void NotEqual(int value);
        void NotEqual(long value);
        void NotEqual(string value);
    }

    public interface ITableQueryFilterConditionBuilder<TValue>
    {
        void Equal(TValue value);
        void GreaterThan(TValue value);
        void GreaterThanOrEqual(TValue value);
        void LessThan(TValue value);
        void LessThanOrEqual(TValue value);
        void NotEqual(TValue value);
    }
}