using CliWrap;
using Handyman.Extensions;
using Handyman.Tools.Outdated.Analyze;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated.IO;

public class ProcessRunner : IProcessRunner
{
    public async Task<ProcessResult> Execute(string targetFilePath, IEnumerable<string> arguments, Verbosity verbosity)
    {
        if (verbosity == Verbosity.Debug)
        {
            DebugMode = true;
        }

        WriteDebugOutput(new[] { targetFilePath }.Concat(arguments).Join(" "));

        var error = new List<string>();
        var output = new List<string>();

        var commandResult = await Cli.Wrap(targetFilePath)
            .WithArguments(arguments)
            .WithStandardErrorPipe(PipeTarget.ToDelegate(s => ProcessOutput(s, error)))
            .WithStandardOutputPipe(PipeTarget.ToDelegate(s => ProcessOutput(s, output)))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

        return new ProcessResult
        {
            ExitCode = commandResult.ExitCode,
            StandardError = error,
            StandardOutput = output
        };
    }

    private static void ProcessOutput(string s, List<string> buffer)
    {
        if (string.IsNullOrWhiteSpace(s))
            return;

        buffer.Add(s);

        WriteDebugOutput(s);
    }

    private static bool? DebugMode;
    private static string DebugPrefix;

    private static void WriteDebugOutput(string s)
    {
        DebugMode ??= Environment.GetEnvironmentVariable("SYSTEM_DEBUG")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true;

        if (DebugMode != true)
        {
            return;
        }

        if (DebugPrefix is null)
        {
            DebugPrefix = string.Empty;

            if (Environment.GetEnvironmentVariable("TF_BUILD")?.Equals("true", StringComparison.OrdinalIgnoreCase) == true)
            {
                DebugPrefix = "##[debug]";
            }
        }

        Console.WriteLine(DebugPrefix + s);
    }
}