using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Outdated.Model
{
    public class ProjectConfig
    {
        public bool Skip { get; set; }
        public bool? IncludeTransitive { get; set; }
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }
}