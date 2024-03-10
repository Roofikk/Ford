using System;
using System.Threading.Tasks;

public static class TaskExtension
{
    public static Task<TResult> RunOnMainThread<TResult>(this Task<TResult> task, Action<TResult> continuetion)
    {
        task.ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
        {
            continuetion?.Invoke(task.Result);
        });

        return task;
    }

    public static Task RunOnMainThread(this Task task, Action continuation)
    {
        task.ConfigureAwait(true).GetAwaiter().OnCompleted(() =>
        {
            continuation?.Invoke();
        });

        return task;
    }
}
