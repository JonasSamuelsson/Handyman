using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal static class TaskExtensions
    {
        internal static ConfiguredTaskAwaitable ConfigureAwait(this Task task)
        {
            return task.ConfigureAwait(AsyncAwaitOptions.ContinueOnCapturedContext);
        }

        internal static ConfiguredTaskAwaitable<T> ConfigureAwait<T>(this Task<T> task)
        {
            return task.ConfigureAwait(AsyncAwaitOptions.ContinueOnCapturedContext);
        }
    }
}