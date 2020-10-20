namespace Handyman.Tools.Docs.Utils
{
    public class Element<TData>
    {
        public TData Data { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }

        public Lines ElementLines { get; set; }
        public Lines ContentLines { get; set; }
    }
}