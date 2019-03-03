namespace Handyman.Mediator.Internals
{
    internal class Providers
    {
        public IEventHandlerProvider EventHandlerProvider { get; set; }
        public IEventPipelineHandlerProvider EventPipelineHandlerProvider { get; set; }
        internal IRequestPipelineHandlerProvider RequestPipelineHandlerProvider { get; set; }
        internal IRequestHandlerProvider RequestHandlerProvider { get; set; }
        internal ServiceProvider ServiceProvider { get; set; }
    }
}