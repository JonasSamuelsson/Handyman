using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Outdated.Model
{
    public class Package
    {
        public string Name { get; set; }
        public string CurrentVersion { get; set; }
        public Dictionary<UpdateSeverity, PackageUpdate> Updates { get; } = new Dictionary<UpdateSeverity, PackageUpdate>();
        public bool IsTransitive { get; set; }
        public PackageDeprecation Deprecation { get; } = new PackageDeprecation();
        public string Info { get; set; } = string.Empty;

        public bool NeedsAttention => !string.IsNullOrEmpty(Info) || Deprecation.IsDeprecated || Updates.Any();
    }
}