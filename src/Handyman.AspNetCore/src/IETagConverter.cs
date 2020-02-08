namespace Handyman.AspNetCore
{
    public interface IETagConverter
    {
        string FromByteArray(byte[] bytes);
        byte[] ToByteArray(string eTag);
    }
}