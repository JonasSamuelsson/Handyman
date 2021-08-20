namespace Handyman.Mediator.Pipeline
{
    internal static class PipelineBuilderComparer
    {
        internal static int Compare(object x, object y)
        {
            return GetOrder(x).CompareTo(GetOrder(y));
        }

        internal static int GetOrder(object o)
        {
            return (o as IOrderedPipelineBuilder)?.Order ?? MediatorDefaults.Order.Default;
        }
    }
}