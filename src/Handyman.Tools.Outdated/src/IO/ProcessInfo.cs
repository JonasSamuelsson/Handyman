using System.Diagnostics;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated.IO
{
    public class ProcessInfo : IProcessInfo
    {
        private readonly Process _process;

        public ProcessInfo(Process process, Task<int> task)
        {
            _process = process;
            Task = task;
        }

        public Task<int> Task { get; }

        public void Close()
        {
            _process.Close();
        }

        public void Dispose()
        {
            _process.Dispose();
        }

        public void Kill(bool entireProcessTree)
        {
            _process.Kill(entireProcessTree);
        }
    }
}