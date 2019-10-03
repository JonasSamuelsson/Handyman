using System.Threading.Tasks;

namespace Handyman.Mediator.Tests
{
    internal static class TaskExtensions
    {
        internal static bool IsCompletedSuccessfully(this Task task)
        {
#if NET472
            return task.Status == TaskStatus.RanToCompletion;
#else
            return task.IsCompletedSuccessfully;
#endif
        }
    }
}