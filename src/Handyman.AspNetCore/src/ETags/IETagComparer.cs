namespace Handyman.AspNetCore.ETags;

public interface IETagComparer
{
    void EnsureEquals(string eTag, byte[] bytes);
    void EnsureEquals(string eTag1, string eTag2);
    bool Equals(string eTag, byte[] bytes);
    bool Equals(string eTag1, string eTag2);
}