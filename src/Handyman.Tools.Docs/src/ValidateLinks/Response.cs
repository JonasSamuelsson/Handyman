namespace Handyman.Tools.Docs.ValidateLinks;

public class Response
{
    public int StatusCode { get; set; }
    public string ReasonPhrase { get; set; }

    public bool IsSuccessStatusCode => StatusCode < 400;
}