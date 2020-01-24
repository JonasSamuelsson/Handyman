using Handyman.Tools.Outdated.Analyze;
using Newtonsoft.Json;

namespace Handyman.Tools.Outdated.Model
{
    public class PackageConfig
    {
        public string Name { get; set; }
        public string IgnoreVersion { get; set; }

        [JsonIgnore] public PackageNameFilter NameFilter { get; set; }
        [JsonIgnore] public PackageVersionFilter VersionFilter { get; set; }
    }
}