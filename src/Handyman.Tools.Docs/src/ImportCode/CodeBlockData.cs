using Handyman.Tools.Docs.Utils;

namespace Handyman.Tools.Docs.ImportCode
{
    public class CodeBlockData
    {
        [Xor]
        public string Id { get; set; }

        public string Language { get; set; }

        [ValueConverter(typeof(LinesValueConverter)), Xor]
        public Lines Lines { get; set; }

        [Required]
        public string Source { get; set; }
    }
}