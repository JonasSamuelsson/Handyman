namespace Handyman.Azure.Cosmos.Table
{
    internal interface ITableQueryFilterNode
    {
        void Add(ITableQueryFilterNode node);
        string Build();
    }
}