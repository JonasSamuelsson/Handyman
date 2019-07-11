namespace Handyman.Azure.Cosmos.Table.Internals
{
    internal interface ITableQueryFilterNode
    {
        void Add(ITableQueryFilterNode node);
        string Build();
    }
}