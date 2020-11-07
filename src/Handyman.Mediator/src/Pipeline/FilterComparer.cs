namespace Handyman.Mediator.Pipeline
{
    internal static class FilterComparer
    {
        internal static int CompareFilters(object x, object y)
        {
            return GetOrder(x).CompareTo(GetOrder(y));
        }

        private static int GetOrder(object o)
        {
            return (o as IOrderedFilter)?.Order ?? MediatorDefaults.Order.Default;
        }
    }
}