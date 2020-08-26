namespace Handyman.DuckTyping
{
    internal static class Utils
    {
        internal static long Combine(int x, int y) => ((long)x << 32) | (uint)y;
    }
}