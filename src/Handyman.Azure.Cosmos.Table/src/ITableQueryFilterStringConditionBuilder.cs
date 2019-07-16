namespace Handyman.Azure.Cosmos.Table
{
    public interface ITableQueryFilterStringConditionBuilder : ITableQueryFilterConditionBuilder<string>
    {
        void StartsWith(string value);
    }
}