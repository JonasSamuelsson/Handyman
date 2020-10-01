using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated.IO
{
    public class ProcessRunner : IProcessRunner
    {
        public IProcessInfo Start(ProcessStartInfo startInfo)
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                Arguments = startInfo.Arguments,
                CreateNoWindow = startInfo.CreateNoWindow,
                Domain = startInfo.Domain,
                ErrorDialog = startInfo.ErrorDialog,
                FileName = startInfo.FileName,
                LoadUserProfile = startInfo.LoadUserProfile,
                Password = startInfo.Password,
                PasswordInClearText = startInfo.PasswordInClearText,
                RedirectStandardError = startInfo.RedirectStandardError || startInfo.StandardErrorHandler != null!,
                RedirectStandardInput = startInfo.RedirectStandardInput,
                RedirectStandardOutput = startInfo.RedirectStandardOutput || startInfo.StandardOutputHandler != null,
                StandardErrorEncoding = startInfo.StandardErrorEncoding,
                StandardInputEncoding = startInfo.StandardInputEncoding,
                StandardOutputEncoding = startInfo.StandardOutputEncoding,
                UseShellExecute = startInfo.UseShellExecute,
                UserName = startInfo.UserName,
                Verb = startInfo.Verb,
                WindowStyle = startInfo.WindowStyle,
                WorkingDirectory = startInfo.WorkingDirectory
            };

            if ((psi.RedirectStandardError || psi.RedirectStandardOutput) && psi.UseShellExecute)
                ; // print error

            foreach (string key in startInfo.EnvironmentVariables.Keys)
            {
                psi.EnvironmentVariables[key] = startInfo.EnvironmentVariables[key];
            }

            var process = new Process { StartInfo = psi };
            var actions = new List<Action>();

            // there has been times where the [Error/Output]DataReceived events has fired after the process has exited
            // adding a little layer of indirection to mitigate that
            var handlers = new Handlers();

            if (startInfo.StandardErrorHandler != null)
            {
                handlers.Error = startInfo.StandardErrorHandler;
                process.ErrorDataReceived += (_, args) => handlers.Error.Invoke(args.Data);
                actions.Add(() => process.BeginErrorReadLine());
            }

            if (startInfo.StandardOutputHandler != null)
            {
                handlers.Output = startInfo.StandardOutputHandler;
                process.OutputDataReceived += (_, args) => handlers.Output.Invoke(args.Data);
                actions.Add(() => process.BeginOutputReadLine());
            }

            var tcs = new TaskCompletionSource<int>();
            process.EnableRaisingEvents = true;
            process.Exited += (_, args) =>
            {
                handlers.Error = delegate { };
                handlers.Output = delegate { };
                tcs.SetResult(process.ExitCode);
            };
            var task = tcs.Task;

            process.Start();

            actions.ForEach(x => x.Invoke());

            return new ProcessInfo(process, task);
        }

        private class Handlers
        {
            public Action<string> Error { get; set; }
            public Action<string> Output { get; set; }
        }
    }
}