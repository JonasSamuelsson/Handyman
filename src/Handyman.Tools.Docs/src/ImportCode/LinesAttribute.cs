namespace Handyman.Tools.Docs.ImportCode
{
    public class LinesAttribute
    {
        public int FromLineNumber { get; set; }
        public int LineCount { get; set; }

        public int FromLineIndex => FromLineNumber - 1;
    }
}