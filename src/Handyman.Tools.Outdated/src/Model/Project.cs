using System.Collections.Generic;

namespace Handyman.Tools.Outdated.Model
{
    public class Project
    {
        public string FullPath { get; set; }
        public string Name { get; set; }
        public string RelativePath { get; set; }
        public List<TargetFramework> TargetFrameworks { get; } = new List<TargetFramework>();
        public ProjectConfig Config { get; set; }
        public List<Error> Errors { get; } = new List<Error>();
    }
}