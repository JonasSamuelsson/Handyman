using CliWrap;
using Handyman.Tools.Outdated.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated.Analyze
{
    public class ProjectUtil
    {
        public async Task Restore(Project project)
        {
            var errors = new List<string>();
            await Cli.Wrap("dotnet")
                .WithArguments(new[] { "restore", project.FullPath })
                .WithStandardErrorPipe(PipeTarget.ToDelegate(s => errors.Add(s)))
                .WithStandardOutputPipe(PipeTarget.Null)
                .ExecuteAsync();

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