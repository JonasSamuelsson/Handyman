using System;

namespace Handyman.Tools.Docs.Utils
{
    public class Disposable : IDisposable
    {
        public Action Action { get; set; }
        public IDisposable InnerDisposable { get; set; }

        public void Dispose()
        {
            Action?.Invoke();
            InnerDisposable?.Dispose();
        }
    }
}