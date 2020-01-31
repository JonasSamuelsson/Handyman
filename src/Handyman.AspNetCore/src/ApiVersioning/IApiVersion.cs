namespace Handyman.AspNetCore.ApiVersioning
{
    public interface IApiVersion
    {
        string Text { get; }

        int CompareTo(IApiVersion other);
        bool IsMatch(IApiVersion other);
    }
}