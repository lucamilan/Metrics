using System.Threading.Tasks;

namespace MiniMetrics.Extensions
{
    public static class TaskExtensions
    {
        public static void ThrowOnError(this Task task)
        {
            if (task.Exception != null)
                throw task.Exception.GetBaseException();
        }
    }
}