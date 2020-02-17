namespace Handyman.AspNetCore.ETags
{
    public interface IETagValidator
    {
        bool IsValidETag(string candidate);
    }
}