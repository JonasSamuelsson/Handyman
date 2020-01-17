namespace Handyman.Tools.Outdated.Analyze.DotnetListPackage
{
    public class DotnetListPackageDependency
    {
        public string Name { get; set; }
        public bool IsTransitive { get; set; }
        public string Current { get; set; }
        public string Available { get; set; }
    }
}