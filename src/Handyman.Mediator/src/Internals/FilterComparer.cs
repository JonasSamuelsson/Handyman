namespace Handyman.Mediator.Internals
{
    internal static class FilterComparer
    {
        internal static int CompareFilters(object x, object y)
        {
            return GetSortOrder(x).CompareTo(GetSortOrder(y));
        }

        private static int GetSortOrder(object o)
        {
            return (o as IOrderedFilter)?.Order ?? 0;
        }
    }
}