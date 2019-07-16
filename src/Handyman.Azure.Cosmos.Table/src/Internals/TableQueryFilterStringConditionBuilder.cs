using Microsoft.Azure.Cosmos.Table;

namespace Handyman.Azure.Cosmos.Table.Internals
{
    internal class TableQueryFilterStringConditionBuilder : TableQueryFilterConditionBuilder<string>, ITableQueryFilterStringConditionBuilder
    {
        public TableQueryFilterStringConditionBuilder(string propertyName, FilterConditionGenerator<string> generator, ITableQueryFilterNode node)
            : base(propertyName, generator, node)
        {
        }

        public void StartsWith(string value)
        {
            var andNode = new AndTableQueryFilterNode();

            andNode.Add(GenerateConditionNode(QueryComparisons.GreaterThanOrEqual, value));

            var length = value.Length - 1;
            var value2 = $"{value.Substring(0, length)}{(char)(value[length] + 1)}";
            andNode.Add(GenerateConditionNode(QueryComparisons.LessThan, value2));

            AddNode(andNode);
        }
    }
}