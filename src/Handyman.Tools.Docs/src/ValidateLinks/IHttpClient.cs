using System.Threading.Tasks;

namespace Handyman.Tools.Docs.ValidateLinks;

public interface IHttpClient
{
    Task<Response> Get(string url);
}