namespace Handyman.Mediator.Samples.WhenAnyRequestHandler
{
    [WhenAny]
    public class Request : IRequest<string>
    {
        public string Text { get; set; }
        public string[,] Matrix { get; set; }
    }
}