using Handyman.Tools.Outdated.IO;
using Handyman.Tools.Outdated.Model;
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

        public void Restore(Project project)
        {
            var errors = new List<string>();

            var info = new ProcessStartInfo
            {
                Arguments = $"restore {project.FullPath}",
                FileName = "dotnet",
                StandardErrorHandler = s => errors.Add(s),
                StandardOutputHandler = delegate { }
            };

            _processRunner.Start(info).Task.Wait();

            errors.RemoveAll(string.IsNullOrWhiteSpace);

            if (errors.Any())
            {
                project.Errors.Add(new Error
                {
                    Stage = "restore",
                    Message = string.Join(Environment.NewLine, errors)
                });
            }
        }
    }
}