namespace Handyman.AspNetCore.ApiVersioning
{
    public interface IApiVersion
    {
        string Text { get; }

        bool IsMatch(IApiVersion other);
    }
}