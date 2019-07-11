namespace Handyman.Azure.Cosmos.Table
{
    internal delegate string FilterConditionGenerator<TValue>(string property, string operation, TValue value);
}