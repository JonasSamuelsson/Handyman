namespace Handyman.AspNetCore.ETags
{
    internal interface IETagConverter
    {
        string FromByteArray(byte[] bytes);
    }
}