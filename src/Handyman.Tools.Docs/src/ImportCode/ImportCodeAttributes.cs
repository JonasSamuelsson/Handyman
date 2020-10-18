using Handyman.Tools.Docs.Utils;

namespace Handyman.Tools.Docs.ImportCode
{
    public class ImportCodeElementAttributes
    {
        [Xor]
        public string Id { get; set; }

        public string Language { get; set; }

        [Xor]
        public LinesAttribute Lines { get; set; }

        [Required]
        public string Source { get; set; }
    }
}