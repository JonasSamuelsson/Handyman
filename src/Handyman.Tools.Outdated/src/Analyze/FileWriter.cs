using Handyman.Tools.Outdated.Model;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Text;

namespace Handyman.Tools.Outdated.Analyze
{
    public abstract class FileWriter : IFileWriter
    {
        private readonly IFileSystem _fileSystem;

        protected FileWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public abstract bool CanHandle(string extension);

        public abstract void Write(string path, IEnumerable<Project> projects);

        protected void WriteToDisk(string path, string content, Encoding encoding)
        {
            var directory = _fileSystem.Path.GetDirectoryName(path);

            if (!_fileSystem.Directory.Exists(directory))
                _fileSystem.Directory.CreateDirectory(directory);

            _fileSystem.File.WriteAllText(path, content, encoding);
        }
    }
}