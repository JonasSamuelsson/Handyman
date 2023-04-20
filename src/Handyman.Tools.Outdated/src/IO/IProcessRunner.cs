using Handyman.Tools.Outdated.Analyze;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated.IO;

public interface IProcessRunner
{
    Task<ProcessResult> Execute(string targetFilePath, IEnumerable<string> arguments, Verbosity verbosity);
}