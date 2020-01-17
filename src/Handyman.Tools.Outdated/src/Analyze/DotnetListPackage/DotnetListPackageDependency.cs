namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class DotnetListPackageDependency
    {
        public string Name { get; set; }
        public bool IsTransitive { get; set; }
        public string CurrentVersion { get; set; }
        public string AvailableVersion { get; set; }
    }
}