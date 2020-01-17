using System.Collections.Generic;
using Handyman.Tools.Outdated.Model;

namespace Handyman.Tools.Outdated.Analyze
{
    public interface IFileWriter
    {
        bool CanHandle(string extension);
        void Write(string path, IEnumerable<Project> projects);
    }
}