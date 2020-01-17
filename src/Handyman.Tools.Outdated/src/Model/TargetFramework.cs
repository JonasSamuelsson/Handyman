using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Model
{
    public class TargetFramework
    {
        public string Name { get; set; }
        public List<Package> Packages { get; } = new List<Package>();
    }
}