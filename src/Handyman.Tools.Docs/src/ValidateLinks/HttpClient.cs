using System.Net.Http;
using System.Threading.Tasks;

namespace Handyman.Tools.Docs.ValidateLinks;

public class HttpClient : IHttpClient
{
    private readonly System.Net.Http.HttpClient _httpClient = new(new HttpClientHandler
    {
        AllowAutoRedirect = true
    });

    public async Task<Response> Get(string url)
    {
        var httpResponse = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Get, url));

        return new Response
        {
            ReasonPhrase = httpResponse.ReasonPhrase,
            StatusCode = (int)httpResponse.StatusCode
        };
    }
}