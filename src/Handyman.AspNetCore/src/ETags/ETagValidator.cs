namespace Handyman.AspNetCore.ETags
{
    internal class ETagValidator : IETagValidator
    {
        public bool IsValidETag(string candidate)
        {
            if (string.IsNullOrEmpty(candidate))
                return false;

            if (candidate == "*")
                return true;

            if (!candidate.EndsWith("\""))
                return false;

            int start;

            if (candidate.StartsWith("W/\""))
            {
                start = 3;
            }
            else if (candidate.StartsWith("\""))
            {
                start = 1;
            }
            else
            {
                return false;
            }

            return candidate.Length > (start + 1);
        }
    }
}