using System;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated.IO
{
    public interface IProcessInfo : IDisposable
    {
        Task<int> Task { get; }
        void Close();
        void Kill(bool entireProcessTree);
    }
}