using System;

namespace Handyman.Azure.Cosmos.Table.Internals
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

        public void Equals(bool value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equals(byte[] value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equals(DateTimeOffset value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equals(double value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equals(Guid value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equals(int value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equals(long value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void Equals(string value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void GreaterThan(bool value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(byte[] value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(DateTimeOffset value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(double value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(Guid value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(int value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(long value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThan(string value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThanOrEquals(bool value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEquals(byte[] value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEquals(DateTimeOffset value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEquals(double value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEquals(Guid value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEquals(int value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEquals(long value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void GreaterThanOrEquals(string value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void LessThan(bool value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(byte[] value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(DateTimeOffset value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(double value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(Guid value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(int value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(long value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThan(string value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThanOrEquals(bool value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEquals(byte[] value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEquals(DateTimeOffset value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEquals(double value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEquals(Guid value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEquals(int value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEquals(long value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void LessThanOrEquals(string value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void NotEquals(bool value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEquals(byte[] value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEquals(DateTimeOffset value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEquals(double value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEquals(Guid value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEquals(int value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEquals(long value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void NotEquals(string value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        public void StartsWith(string value)
        {
            var andNode = new AndTableQueryFilterNode();

            andNode.Add(GenerateConditionNode(QueryComparisons.GreaterThanOrEqual, value, FilterConditionGenerator.GenerateFilterConditionForString));

            var length = value.Length - 1;
            var value2 = $"{value.Substring(0, length)}{(char)(value[length] + 1)}";
            andNode.Add(GenerateConditionNode(QueryComparisons.LessThan, value2, FilterConditionGenerator.GenerateFilterConditionForString));

            _node.Add(andNode);
        }

        private void AddFilterCondition(string operation, bool value)
        {
            AddFilterCondition(operation, value, FilterConditionGenerator.GenerateFilterConditionForBool);
        }

        private void AddFilterCondition(string operation, byte[] value)
        {
            AddFilterCondition(operation, value, FilterConditionGenerator.GenerateFilterConditionForBinary);
        }

        private void AddFilterCondition(string operation, DateTimeOffset value)
        {
            AddFilterCondition(operation, value, FilterConditionGenerator.GenerateFilterConditionForDate);
        }

        private void AddFilterCondition(string operation, double value)
        {
            AddFilterCondition(operation, value, FilterConditionGenerator.GenerateFilterConditionForDouble);
        }

        private void AddFilterCondition(string operation, Guid value)
        {
            AddFilterCondition(operation, value, FilterConditionGenerator.GenerateFilterConditionForGuid);
        }

        private void AddFilterCondition(string operation, int value)
        {
            AddFilterCondition(operation, value, FilterConditionGenerator.GenerateFilterConditionForInt);
        }

        private void AddFilterCondition(string operation, long value)
        {
            AddFilterCondition(operation, value, FilterConditionGenerator.GenerateFilterConditionForLong);
        }

        private void AddFilterCondition(string operation, string value)
        {
            AddFilterCondition(operation, value, FilterConditionGenerator.GenerateFilterConditionForString);
        }

        private void AddFilterCondition<TValue>(string operation, TValue value, FilterConditionGeneratorDelegate<TValue> generator)
        {
            _node.Add(GenerateConditionNode(operation, value, generator));
        }

        private ConditionTableQueryFilterNode GenerateConditionNode<TValue>(string operation, TValue value, FilterConditionGeneratorDelegate<TValue> generator)
        {
            return new ConditionTableQueryFilterNode(generator.Invoke(_propertyName, operation, value));
        }
    }

    internal class TableQueryFilterConditionBuilder<TValue> : ITableQueryFilterConditionBuilder<TValue>
    {
        private readonly string _propertyName;
        private readonly FilterConditionGeneratorDelegate<TValue> _generator;
        private readonly ITableQueryFilterNode _node;

        public TableQueryFilterConditionBuilder(string propertyName, FilterConditionGeneratorDelegate<TValue> generator, ITableQueryFilterNode node)
        {
            _propertyName = propertyName;
            _generator = generator;
            _node = node;
        }

        public void Equals(TValue value)
        {
            AddFilterCondition(QueryComparisons.Equal, value);
        }

        public void GreaterThan(TValue value)
        {
            AddFilterCondition(QueryComparisons.GreaterThan, value);
        }

        public void GreaterThanOrEquals(TValue value)
        {
            AddFilterCondition(QueryComparisons.GreaterThanOrEqual, value);
        }

        public void LessThan(TValue value)
        {
            AddFilterCondition(QueryComparisons.LessThan, value);
        }

        public void LessThanOrEquals(TValue value)
        {
            AddFilterCondition(QueryComparisons.LessThanOrEqual, value);
        }

        public void NotEquals(TValue value)
        {
            AddFilterCondition(QueryComparisons.NotEqual, value);
        }

        private void AddFilterCondition(string operation, TValue value)
        {
            AddNode(GenerateConditionNode(operation, value));
        }

        protected void AddNode(ITableQueryFilterNode node)
        {
            _node.Add(node);
        }

        protected ITableQueryFilterNode GenerateConditionNode(string operation, TValue value)
        {
            var condition = _generator.Invoke(_propertyName, operation, value);
            return new ConditionTableQueryFilterNode(condition);
        }
    }
}