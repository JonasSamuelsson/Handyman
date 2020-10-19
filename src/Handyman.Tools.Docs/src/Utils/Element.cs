namespace Handyman.Tools.Docs.Utils
{
    public class Element<TData>
    {
        public TData Data { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int ElementLineIndex { get; set; }
        public int ElementLineCount { get; set; }
        public int ContentLineIndex { get; set; }
        public int ContentLineCount { get; set; }

        public bool HasContent => ContentLineCount != 0;
    }
}