namespace Handyman.Mediator.Pipeline
{
    internal static class PipelineBuilderComparer
    {
        internal static int Compare(object x, object y)
        {
            return GetSortOrder(x).CompareTo(GetSortOrder(y));
        }

        private static int GetSortOrder(object o)
        {
            return (o as IOrderedPipelineBuilder)?.ExecutionOrder ?? 0;
        }
    }
}