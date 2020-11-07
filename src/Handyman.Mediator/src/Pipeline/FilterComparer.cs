namespace Handyman.Mediator.Pipeline
{
    internal static class FilterComparer
    {
        internal static int CompareFilters(object x, object y)
        {
            return GetExecutionOrder(x).CompareTo(GetExecutionOrder(y));
        }

        private static int GetExecutionOrder(object o)
        {
            return (o as IOrderedFilter)?.Order ?? Defaults.Order.Default;
        }
    }
}