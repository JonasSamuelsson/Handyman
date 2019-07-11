using Microsoft.Azure.Cosmos.Table;
using System;

namespace Handyman.Azure.Cosmos.Table
{
    internal class TableQueryFilterConditionBuilder : ITableQueryFilterConditionBuilder
    {
        private readonly string _propertyName;
        private readonly ITableQueryFilterNode _node;

        public TableQueryFilterConditionBuilder(string propertyName, ITableQueryFilterNode node)
        {
            _propertyName = propertyName;
            _node = node;
        }

        public void Equal(bool value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equal(byte[] value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equal(DateTimeOffset value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equal(double value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equal(Guid value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equal(int value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equal(long value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equal(string value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void GreaterThan(bool value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(byte[] value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(DateTimeOffset value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(double value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(Guid value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(int value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(long value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(string value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThanOrEqual(bool value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEqual(byte[] value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEqual(DateTimeOffset value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEqual(double value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEqual(Guid value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEqual(int value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEqual(long value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEqual(string value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void LessThan(bool value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(byte[] value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(DateTimeOffset value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(double value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(Guid value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(int value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(long value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(string value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThanOrEqual(bool value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEqual(byte[] value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEqual(DateTimeOffset value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEqual(double value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEqual(Guid value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEqual(int value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEqual(long value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEqual(string value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void NotEqual(bool value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEqual(byte[] value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEqual(DateTimeOffset value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEqual(double value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEqual(Guid value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEqual(int value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEqual(long value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEqual(string value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        private void GenerateFilterCondition(string operation, bool value)
        {
            GenerateFilterCondition(operation, value, TableQuery.GenerateFilterConditionForBool);
        }

        private void GenerateFilterCondition(string operation, byte[] value)
        {
            GenerateFilterCondition(operation, value, TableQuery.GenerateFilterConditionForBinary);
        }

        private void GenerateFilterCondition(string operation, DateTimeOffset value)
        {
            GenerateFilterCondition(operation, value, TableQuery.GenerateFilterConditionForDate);
        }

        private void GenerateFilterCondition(string operation, double value)
        {
            GenerateFilterCondition(operation, value, TableQuery.GenerateFilterConditionForDouble);
        }

        private void GenerateFilterCondition(string operation, Guid value)
        {
            GenerateFilterCondition(operation, value, TableQuery.GenerateFilterConditionForGuid);
        }

        private void GenerateFilterCondition(string operation, int value)
        {
            GenerateFilterCondition(operation, value, TableQuery.GenerateFilterConditionForInt);
        }

        private void GenerateFilterCondition(string operation, long value)
        {
            GenerateFilterCondition(operation, value, TableQuery.GenerateFilterConditionForLong);
        }

        private void GenerateFilterCondition(string operation, string value)
        {
            GenerateFilterCondition(operation, value, TableQuery.GenerateFilterCondition);
        }

        private void GenerateFilterCondition<TValue>(string operation, TValue value, FilterConditionGenerator<TValue> generator)
        {
            _node.Add(new TableQueryFilterConditionNode(generator.Invoke(_propertyName, operation, value)));
        }
    }

    internal class TableQueryFilterConditionBuilder<TValue> : ITableQueryFilterConditionBuilder<TValue>
    {
        private readonly string _propertyName;
        private readonly FilterConditionGenerator<TValue> _generator;
        private readonly ITableQueryFilterNode _node;

        public TableQueryFilterConditionBuilder(string propertyName, FilterConditionGenerator<TValue> generator, ITableQueryFilterNode node)
        {
            _propertyName = propertyName;
            _generator = generator;
            _node = node;
        }

        public void Equal(TValue value)
        {
            GenerateFilterCondition(QueryComparisons.Equal, value);
        }

        public void GreaterThan(TValue value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThanOrEqual(TValue value)
        {
            GenerateFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void LessThan(TValue value)
        {
            GenerateFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThanOrEqual(TValue value)
        {
            GenerateFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void NotEqual(TValue value)
        {
            GenerateFilterCondition(QueryComparisons.NotEqual, value);
        }

        private void GenerateFilterCondition(string operation, TValue value)
        {
            _node.Add(new TableQueryFilterConditionNode(_generator.Invoke(_propertyName, operation, value)));
        }
    }
}