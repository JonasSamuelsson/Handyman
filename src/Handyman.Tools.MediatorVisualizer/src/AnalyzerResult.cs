namespace Handyman.Tools.MediatorVisualizer
{
    public class AnalyzerResult
    {
        public Set Handlers { get; set; } = new Set();
        public Set EntryPoints { get; set; } = new Set();

        public Dictionary EventHandledBy { get; set; } = new Dictionary();
        //public Dictionary EventPublishedBy { get; set; } = new Dictionary();
        public Dictionary DispatcherPublishes { get; set; } = new Dictionary();
        public Dictionary DispatcherSends { get; set; } = new Dictionary();
        public Dictionary RequestHandledBy { get; set; } = new Dictionary();
        //public Dictionary RequestSentBy { get; set; } = new Dictionary();
    }
}