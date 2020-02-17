namespace Handyman.AspNetCore.ETags
{
    public interface IETagConverter
    {
        string FromByteArray(byte[] bytes);
        byte[] ToByteArray(string eTag);
    }
}