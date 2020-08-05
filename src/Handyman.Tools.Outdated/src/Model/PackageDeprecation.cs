namespace Handyman.Tools.Outdated.Model
{
    public class PackageDeprecation
    {
        public string Alternative { get; set; } = string.Empty;
        public bool IsDeprecated { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}