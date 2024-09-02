namespace Handyman.AspNetCore.ETags.Internals
{
    internal class ETagValidator : IETagValidator
    {
        public bool IsValidETag(string candidate)
        {
            return ETagUtility.IsValidETag(candidate);
        }
    }
}