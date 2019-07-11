namespace Handyman.Azure.Cosmos.Table.Internals
{
    internal delegate string FilterConditionGenerator<TValue>(string property, string operation, TValue value);
}