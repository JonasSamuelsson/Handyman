namespace Handyman.AspNetCore.ApiVersioning;

public interface IApiVersionParser
{
    bool TryParse(string candidate, out IApiVersion version);
}