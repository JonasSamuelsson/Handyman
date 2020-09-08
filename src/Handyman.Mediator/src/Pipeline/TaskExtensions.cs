using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal static class TaskExtensions
    {
        internal static ConfiguredTaskAwaitable WithGloballyConfiguredAwait(this Task task)
        {
            return task.ConfigureAwait(MediatorAwaiterOptions.ContinueOnCapturedContext);
        }

        internal static ConfiguredTaskAwaitable<T> WithGloballyConfiguredAwait<T>(this Task<T> task)
        {
            return task.ConfigureAwait(MediatorAwaiterOptions.ContinueOnCapturedContext);
        }
    }
}