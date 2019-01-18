namespace Handyman.Mediator.Internals
{
    internal class Providers
    {
        internal ServiceProvider ServiceProvider { get; set; }
        internal IRequestPipelineHandlerProvider RequestPipelineHandlerProvider { get; set; }
        internal IRequestHandlerProvider RequestHandlerProvider { get; set; }
    }
}