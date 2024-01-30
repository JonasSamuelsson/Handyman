namespace Handyman.Azure.Cosmos.Table.Internals
{
    internal delegate string FilterConditionGeneratorDelegate<TValue>(string property, string operation, TValue value);
}