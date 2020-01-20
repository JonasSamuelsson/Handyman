using Handyman.Tools.Outdated.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ProjectUtil
    {
        private readonly IProcessRunner _processRunner;

        public ProjectUtil(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        public void Restore(string path)
        {
            var errors = new List<string>();

            var info = new ProcessStartInfo
            {
                Arguments = $"restore {path}",
                FileName = "dotnet",
                StandardErrorHandler = s => errors.Add(s),
                StandardOutputHandler = delegate { }
            };

            _processRunner.Start(info).Task.Wait();

            errors.RemoveAll(string.IsNullOrWhiteSpace);

            if (errors.Any())
            {
                throw new ApplicationException(string.Join(Environment.NewLine, errors));
            }
        }
    }
}