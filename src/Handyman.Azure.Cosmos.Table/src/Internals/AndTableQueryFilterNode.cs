namespace Handyman.Azure.Cosmos.Table.Internals;

internal class AndTableQueryFilterNode : CompositionTableQueryFilterNode
{
    public AndTableQueryFilterNode() : base(TableOperators.And)
    {
    }
}