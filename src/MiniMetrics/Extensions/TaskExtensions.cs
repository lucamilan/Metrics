using System;
using System.Threading;
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

        public static Task ContinueWithOrThrow(this Task task,
                                               Action<Task> continuationAction,
                                               CancellationToken token)
        {
            return task.ContinueWith(_ =>
                                     {
                                         _.ThrowOnError();
                                         token.ThrowIfCancellationRequested();
                                         continuationAction(_);
                                     },
                                     token);
        }

        public static Task<TResult> ContinueWithOrThrow<TResult>(this Task task,
                                                                 Func<Task, TResult> continuationFunction,
                                                                 CancellationToken token)
        {
            return task.ContinueWith(_ =>
                                     {
                                         _.ThrowOnError();
                                         token.ThrowIfCancellationRequested();
                                         return continuationFunction(_);
                                     },
                                     token);
        }

        public static Task<TNewResult> ContinueWithOrThrow<TNewResult, TResult>(this Task<TResult> task,
                                                                                Func<Task<TResult>, TNewResult> continuationFunction)
        {
            return task.ContinueWith(_ =>
                                     {
                                         _.ThrowOnError();
                                         return continuationFunction(_);
                                     });
        }

        public static Task<TResult> ContinueWithOrThrow<TResult>(this Task task,
                                                                 Func<Task, TResult> continuationFunction)
        {
            return task.ContinueWith(_ =>
                                     {
                                         _.ThrowOnError();
                                         return continuationFunction(_);
                                     });
        }
    }
}