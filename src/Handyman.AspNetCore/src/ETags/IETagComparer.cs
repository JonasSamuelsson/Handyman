namespace Handyman.AspNetCore.ETags
{
    public interface IETagComparer
    {
        bool Equals(string eTag, byte[] bytes);
        bool Equals(string eTag1, string eTag2);
    }
}