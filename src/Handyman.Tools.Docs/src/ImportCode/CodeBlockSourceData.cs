using Handyman.Tools.Docs.Utils;

namespace Handyman.Tools.Docs.ImportCode
{
    public class CodeBlockSourceData : ElementData
    {
        public string Id { get; set; }

        [ValueConverter(typeof(LinesValueConverter))]
        public Lines Lines { get; set; }
    }
}