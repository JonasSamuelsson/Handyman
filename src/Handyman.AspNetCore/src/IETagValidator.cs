namespace Handyman.AspNetCore
{
    public interface IETagValidator
    {
        bool IsValidETag(string eTag);
    }
}