using System;

namespace Handyman.Tools.Docs.ImportCode
{
    public class ImportCodeElementAttributes
    {
        public string Source { get; set; }
        public string Id { get; set; }
        public int? FromLineNumber { get; set; }
        public int? ToLineNumber { get; set; }

        public int? FromLineIndex => FromLineNumber - 1;
        public int? LineCount => throw new NotImplementedException();
        public int? ToLineIndex => ToLineNumber - 1;
    }
}