using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !WINDOWS_PHONE_7

using System.Threading;
using System.Threading.Tasks;

namespace FrodLib.Extensions
{
    public static class TaskExtensions
    {
        // Inspired by http://blogs.msdn.com/b/pfxteam/archive/2010/11/21/10094564.aspx


        public static Task Then(this Task task, Action<Task> next, TaskScheduler scheduler = null, CancellationToken cancellationToken = new CancellationToken(), TaskContinuationOptions continuationOption = TaskContinuationOptions.None)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (next == null) throw new ArgumentNullException("next");

            SetDefaultTaskScheduler(ref scheduler);
            var tcs = new TaskCompletionSource<AsyncVoid>();

            task.ContinueWith(previousTask =>
            {
                if (previousTask.IsFaulted) tcs.TrySetException(previousTask.Exception);
                else if (previousTask.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        next(previousTask);
                        tcs.TrySetResult(default(AsyncVoid));
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, cancellationToken, continuationOption, scheduler);

            return tcs.Task;
        }

        public static Task Then(this Task task, Func<Task, Task> next, TaskScheduler scheduler = null, CancellationToken cancellationToken = new CancellationToken(), TaskContinuationOptions continuationOption = TaskContinuationOptions.None)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (next == null) throw new ArgumentNullException("next");

            SetDefaultTaskScheduler(ref scheduler);
            var tcs = new TaskCompletionSource<AsyncVoid>();

            task.ContinueWith(previousTask =>
            {
                if (previousTask.IsFaulted) tcs.TrySetException(previousTask.Exception);
                else if (previousTask.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        next(previousTask).ContinueWith(nextTask =>
                        {
                            if (nextTask.IsFaulted) tcs.TrySetException(nextTask.Exception);
                            else if (nextTask.IsCanceled) tcs.TrySetCanceled();
                            else tcs.TrySetResult(default(AsyncVoid));
                        }, cancellationToken, continuationOption, scheduler);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, cancellationToken, continuationOption, scheduler);

            return tcs.Task;
        }

        public static Task<TNextResult> Then<TNextResult>(this Task task, Func<Task, TNextResult> next, TaskScheduler scheduler = null, CancellationToken cancellationToken = new CancellationToken(), TaskContinuationOptions continuationOption = TaskContinuationOptions.None)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (next == null) throw new ArgumentNullException("next");

            SetDefaultTaskScheduler(ref scheduler);
            var tcs = new TaskCompletionSource<TNextResult>();

            task.ContinueWith(previousTask =>
            {
                if (previousTask.IsFaulted) tcs.TrySetException(previousTask.Exception);
                else if (previousTask.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        tcs.TrySetResult(next(previousTask));
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, cancellationToken, continuationOption, scheduler);

            return tcs.Task;
        }

        public static Task<TNextResult> Then<TNextResult>(this Task task, Func<Task, Task<TNextResult>> next, TaskScheduler scheduler = null, CancellationToken cancellationToken = new CancellationToken(), TaskContinuationOptions continuationOption = TaskContinuationOptions.None)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (next == null) throw new ArgumentNullException("next");

            SetDefaultTaskScheduler(ref scheduler);
            var tcs = new TaskCompletionSource<TNextResult>();

            task.ContinueWith(previousTask =>
            {
                if (previousTask.IsFaulted) tcs.TrySetException(previousTask.Exception);
                else if (previousTask.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        next(previousTask).ContinueWith(nextTask =>
                        {
                            if (nextTask.IsFaulted) tcs.TrySetException(nextTask.Exception);
                            else if (nextTask.IsCanceled) tcs.TrySetCanceled();
                            else
                            {
                                try
                                {
                                    tcs.TrySetResult(nextTask.Result);
                                }
                                catch (Exception ex)
                                {
                                    tcs.TrySetException(ex);
                                }
                            }
                        }, cancellationToken, continuationOption, scheduler);
                    }

                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, cancellationToken, continuationOption, scheduler);

            return tcs.Task;
        }

        public static Task Then<TResult>(this Task<TResult> task, Action<Task<TResult>> next, TaskScheduler scheduler = null, CancellationToken cancellationToken = new CancellationToken(), TaskContinuationOptions continuationOption = TaskContinuationOptions.None)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (next == null) throw new ArgumentNullException("next");
            var tcs = new TaskCompletionSource<AsyncVoid>();

            SetDefaultTaskScheduler(ref scheduler);

            task.ContinueWith(previousTask =>
            {
                if (previousTask.IsFaulted) tcs.TrySetException(previousTask.Exception);
                else if (previousTask.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        next(previousTask);

                        tcs.TrySetResult(default(AsyncVoid));
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, cancellationToken, continuationOption, scheduler);

            return tcs.Task;
        }

        public static Task Then<TResult>(this Task<TResult> task, Func<Task<TResult>, Task> next, TaskScheduler scheduler = null, CancellationToken cancellationToken = new CancellationToken(), TaskContinuationOptions continuationOption = TaskContinuationOptions.None)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (next == null) throw new ArgumentNullException("next");

            SetDefaultTaskScheduler(ref scheduler);
            var tcs = new TaskCompletionSource<AsyncVoid>();

            task.ContinueWith(previousTask =>
            {
                if (previousTask.IsFaulted) tcs.TrySetException(previousTask.Exception);
                else if (previousTask.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        next(previousTask).ContinueWith(nextTask =>
                        {
                            if (nextTask.IsFaulted) tcs.TrySetException(nextTask.Exception);
                            else if (nextTask.IsCanceled) tcs.TrySetCanceled();
                            else tcs.TrySetResult(default(AsyncVoid));
                        }, cancellationToken, continuationOption, scheduler);
                    }

                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, cancellationToken, continuationOption, scheduler);

            return tcs.Task;
        }


        public static Task<TNextResult> Then<TResult, TNextResult>(this Task<TResult> task, Func<Task<TResult>, TNextResult> next, TaskScheduler scheduler = null, CancellationToken cancellationToken = new CancellationToken(), TaskContinuationOptions continuationOption = TaskContinuationOptions.None)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (next == null) throw new ArgumentNullException("next");

            SetDefaultTaskScheduler(ref scheduler);
            var tcs = new TaskCompletionSource<TNextResult>();

            task.ContinueWith(previousTask =>
            {
                if (previousTask.IsFaulted) tcs.TrySetException(previousTask.Exception);
                else if (previousTask.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        tcs.TrySetResult(next(previousTask));
                    }

                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, cancellationToken, continuationOption, scheduler);

            return tcs.Task;
        }


        public static Task<TNextResult> Then<TResult, TNextResult>(this Task<TResult> task, Func<Task<TResult>, Task<TNextResult>> next, TaskScheduler scheduler = null, CancellationToken cancellationToken = new CancellationToken(), TaskContinuationOptions continuationOption = TaskContinuationOptions.None)
        {
            if (task == null) throw new ArgumentNullException("task");
            if (next == null) throw new ArgumentNullException("next");

            SetDefaultTaskScheduler(ref scheduler);
            var tcs = new TaskCompletionSource<TNextResult>();

            task.ContinueWith(previousTask =>
            {
                if (previousTask.IsFaulted) tcs.TrySetException(previousTask.Exception);
                else if (previousTask.IsCanceled) tcs.TrySetCanceled();
                else
                {
                    try
                    {
                        next(previousTask).ContinueWith(nextTask =>
                        {
                            if (nextTask.IsFaulted) tcs.TrySetException(nextTask.Exception);
                            else if (nextTask.IsCanceled) tcs.TrySetCanceled();
                            else
                            {
                                try
                                {
                                    tcs.TrySetResult(nextTask.Result);
                                }

                                catch (Exception ex)
                                {
                                    tcs.TrySetException(ex);
                                }
                            }
                        }, cancellationToken, continuationOption, scheduler);
                    }

                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }
                }
            }, cancellationToken, continuationOption, scheduler);

            return tcs.Task;
        }

        private static void SetDefaultTaskScheduler(ref TaskScheduler scheduler)
        {
            if (scheduler == null)
            {
                scheduler = TaskScheduler.Default;
            }
        }

        /// <summary>
        /// Analogous to the finally block in a try/finally
        /// </summary>
        public static void Finally(this Task task, Action<Exception> exceptionHandler, Action finalAction = null)
        {
            task.ContinueWith(t =>
            {
                if (finalAction != null) finalAction();

                if (t.IsCanceled || !t.IsFaulted || exceptionHandler == null) return;

                var innerException = t.Exception.Flatten().InnerExceptions.FirstOrDefault();

                t.Exception.Handle(e =>
                {
                    exceptionHandler(innerException ?? t.Exception);
                    return true;
                });
            });
        }

        /// <summary>
        /// Analogous to the finally block in a try/finally
        /// </summary>
        public static void Finally(this Task task, TaskScheduler scheduler, Action<Exception> exceptionHandler, Action finalAction = null)
        {
            task.ContinueWith(t =>
            {
                if (finalAction != null) finalAction();

                if (t.IsCanceled || !t.IsFaulted || exceptionHandler == null) return;

                var innerException = t.Exception.Flatten().InnerExceptions.FirstOrDefault();


                t.Exception.Handle(e =>
                {
                    exceptionHandler(innerException ?? t.Exception);
                    return true;
                });
            }, scheduler);
        }



        /// <summary>
        /// Analogous to the finally block in a try/finally
        /// </summary>
        public static void Finally<TNextResult>(this Task<TNextResult> task, Action<Exception> exceptionHandler, Action<TNextResult> finalAction = null)
        {
            task.ContinueWith(t =>
            {
                if (finalAction != null) finalAction(t.Result);
                if (t.IsCanceled || !t.IsFaulted || exceptionHandler == null) return;
                var innerException = t.Exception.Flatten().InnerExceptions.FirstOrDefault();

                t.Exception.Handle(e =>
                {
                    exceptionHandler(innerException ?? t.Exception);
                    return true;
                });
            });
        }

        /// <summary>
        /// Analogous to the finally block in a try/finally
        /// </summary>
        public static void Finally<TNextResult>(this Task<TNextResult> task, TaskScheduler scheduler, Action<Exception> exceptionHandler, Action<TNextResult> finalAction = null)
        {
            task.ContinueWith(t =>
            {
                if (finalAction != null) finalAction(t.Result);

                if (t.IsCanceled || !t.IsFaulted || exceptionHandler == null) return;

                var innerException = t.Exception.Flatten().InnerExceptions.FirstOrDefault();

                t.Exception.Handle(e =>
                {
                    exceptionHandler(innerException ?? t.Exception);
                    return true;
                });
            }, scheduler);
        }

        private struct AsyncVoid
        {

            // Based on Brad Wilson's idea, to simulate a non-generic TaskCompletionSource

            // http://bradwilson.typepad.com/blog/2012/04/tpl-and-servers-pt4.html

        }
    }
}

#endif
