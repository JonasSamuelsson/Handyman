namespace Handyman.Azure.Cosmos.Table.Internals;

internal class OrTableQueryFilterNode : CompositionTableQueryFilterNode
{
    public OrTableQueryFilterNode() : base(TableOperators.Or)
    {
    }
}