using Handyman.Tools.Outdated.IO;
using Handyman.Tools.Outdated.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ProjectUtil
    {
        private readonly IProcessRunner _processRunner;

        public ProjectUtil(IProcessRunner processRunner)
        {
            _processRunner = processRunner;
        }

        public async Task Restore(Project project, Verbosity verbosity)
        {
            var result = await _processRunner.Execute("dotnet", new[] { "restore", project.FullPath }, verbosity);

            if (result.StandardError.Any())
            {
                project.Errors.Add(new Error
                {
                    Stage = "restore",
                    Message = string.Join(Environment.NewLine, result.StandardError)
                });
            }
        }
    }
}