using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnrealEngine.Runtime.Native;

namespace UnrealEngine.Runtime
{
    // Note: For callers of synchronous RunXXX functions; deadlocks will likely occur if one core thread calls
    // into another core thread synchronously. That includes internal functions which make synchronous calls to
    // the game thread (FTicker, Coroutine). (core threads: game/rendering/RHI)
    //
    // Example of making calls to game thread dependent functions from the rendering thread:
    // FThreading.RunOnGameThreadAsync(() => FTicker.AddTicker(MyTicker));
    // FThreading.RunOnGameThreadAsync(() => EngineLoop.StartCoroutine(null, MyFunc));
    //
    // Possibly some type of an messaging / event system would need to be created if you want to constantly make
    // calls between core threads.


    // This isn't a native class (rename this?). This wraps global functions in ThreadingBase.h
    // Engine\Source\Runtime\Core\Public\HAL\ThreadingBase.h

    public static class FThreading
    {
        /// <summary>
        /// Returns true if called from the game thread.
        /// </summary>
        public static bool IsInGameThread()
        {
            return Native_FThreading.IsInGameThread();
        }

        /// <summary>
        /// Returns true if called from the slate thread, and not merely a thread calling slate functions.
        /// </summary>
        public static bool IsInSlateThread()
        {
            return Native_FThreading.IsInSlateThread();
        }

        /// <summary>
        /// Returns true if called from the rendering thread, or if called from ANY thread during single threaded rendering
        /// </summary>
        public static bool IsInRenderingThread()
        {
            return Native_FThreading.IsInRenderingThread();
        }

        /// <summary>
        /// Returns true if called from the rendering thread, or if called from ANY thread that isn't the game thread, except that during single threaded rendering the game thread is ok too.
        /// </summary>
        /// <returns></returns>
        public static bool IsInParallelRenderingThread()
        {
            return Native_FThreading.IsInParallelRenderingThread();
        }

        /// <summary>
        /// Returns true if called from the rendering thread.
        /// Unlike IsInRenderingThread, this will always return false if we are running single threaded. It only returns true if this is actually a separate rendering thread. Mostly useful for checks
        /// </summary>
        public static bool IsInActualRenderingThread()
        {
            return Native_FThreading.IsInActualRenderingThread();
        }

        /// <summary>
        /// Returns true if called from the async loading thread if it's enabled, otherwise if called from game thread while is async loading code.
        /// </summary>
        public static bool IsInAsyncLoadingThread()
        {
            return Native_FThreading.IsInAsyncLoadingThread();
        }

        /// <summary>
        /// Returns true if called from the RHI thread, or if called from ANY thread during single threaded rendering
        /// </summary>
        public static bool IsInRHIThread()
        {
            return Native_FThreading.IsInRHIThread();
        }

        /// <summary>
        /// Returns true if rendering thread and game thread are the same
        /// </summary>
        public static bool IsRenderingThreadGameThread()
        {
            return Native_FThreading.IsRenderingThreadGameThread();
        }

        /// <summary>
        /// Runs a function on the game thread synchronously
        /// </summary>
        public static void RunOnGameThread(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.GameThread, true);
        }

        /// <summary>
        /// Runs a function on the game thread asynchronously
        /// </summary>
        public static void RunOnGameThreadAsync(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.GameThread, false);
        }

        /// <summary>
        /// Runs a function on the RHI thread synchronously
        /// </summary>
        public static void RunOnRHIThread(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.RHIThread, true);
        }

        /// <summary>
        /// Runs a function on the RHI thread asynchronously
        /// </summary>
        public static void RunOnRHIThreadAsync(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.RHIThread, false);
        }

        /// <summary>
        /// Runs a function on the rendering thread synchronously
        /// </summary>
        public static void RunOnRenderingThread(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.RenderingThread, true);
        }

        /// <summary>
        /// Runs a function on the rendering thread asynchronously
        /// </summary>
        public static void RunOnRenderingThreadAsync(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.RenderingThread, false);
        }

        /// <summary>
        /// Runs a function on any thread synchronously (this is a useless function)
        /// </summary>
        internal static void RunOnAnyThread(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.AnyThread, true);
        }

        /// <summary>
        /// Runs a function on any thread asynchronously (no real reason to use this - just use regular C# threading)
        /// </summary>
        internal static void RunOnAnyThreadAsync(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.AnyThread, false);
        }

        internal static void RunUnloader(FSimpleDelegate func)
        {
            Run(func, EAsyncThreadType.GameThreadUnloadIgnore, true);
        }

        private static void Run(FSimpleDelegate func, EAsyncThreadType threadType, bool waitForComplete)
        {
            switch (threadType)
            {
                case EAsyncThreadType.GameThreadUnloadIgnore:
                case EAsyncThreadType.GameThread:
                    if (IsInGameThread())
                    {
                        func();
                        return;
                    }
                    break;

                case EAsyncThreadType.RHIThread:
                    if (IsInRHIThread())
                    {
                        func();
                        return;
                    }
                    break;                

                case EAsyncThreadType.RenderingThread:
                    if (IsInRenderingThread())
                    {
                        func();
                        return;
                    }
                    break;
            }

            if (HotReload.IsUnloading)
            {
                return;
            }

            AsyncCallback callback = new AsyncCallback(func, threadType);
            lock (callbacks)
            {
                callbacks.Add(callback);
            }
            Native_FAsync.AsyncTask(callback.Run, threadType);
            if (waitForComplete)
            {
                callback.WaitForComplete();
            }
            else
            {
                callback.Dispose();
            }
        }

        /// <summary>
        /// Simple async callback class for holding onto the callback function and waiting for synchronous calls
        /// </summary>
        class AsyncCallback : IDisposable
        {
            private FSimpleDelegate callback;
            private bool disposed = false;
            private AutoResetEvent waitHandle = new AutoResetEvent(false);

            public EAsyncThreadType ThreadType { get; private set; }

            public bool IsCoreThread
            {
                get
                {
                    switch (ThreadType)
                    {
                        case EAsyncThreadType.GameThreadUnloadIgnore:
                        case EAsyncThreadType.GameThread:
                        case EAsyncThreadType.RHIThread:
                        case EAsyncThreadType.RenderingThread:
                            return true;
                        default:
                            return false;

                    }
                }
            }

            public AsyncCallback(FSimpleDelegate callback, EAsyncThreadType threadType)
            {
                this.callback = callback;
                ThreadType = threadType;
            }

            public void Run()
            {
                if (!HotReload.IsUnloading)
                {
                    callback();
                }

                if (!disposed)
                {
                    try
                    {
                        waitHandle.Set();
                    }
                    catch
                    {
                    }
                }

                lock (callbacks)
                {
                    callbacks.Remove(this);
                }
            }

            public void WaitForComplete()
            {
                WaitForComplete(Timeout.Infinite);
            }

            public void WaitForComplete(int timeout)
            {
                if (!disposed)
                {
                    try
                    {
                        waitHandle.WaitOne(timeout);
                    }
                    catch
                    {
                    }
                    Dispose();
                }
            }

            public void Dispose()
            {
                if (!disposed)
                {
                    disposed = true;
                    try
                    {
                        waitHandle.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        // Hold onto a list of callbacks to stop the function delegate from being GCed
        private static List<AsyncCallback> callbacks = new List<AsyncCallback>();
        
        public static void OnUnload()
        {
            // Note: this needs to be done outside of the game thread as some of these threads wont exit
            // with the game thread being blocked

            lock (callbacks)
            {
                // Is there anything we can do here to safetly ensure all callbacks close?

                // For now wait for the callback to complete assuming the target thread is
                // one of the core threads which shouldn't hang forever (game/RHI/rendering)
                // Possibly set a timeout?

                foreach (AsyncCallback callback in callbacks)
                {
                    if (callback.ThreadType == EAsyncThreadType.GameThreadUnloadIgnore)
                    {
                        // Skip the unloader
                        continue;
                    }

                    if (callback.IsCoreThread)
                    {
                        callback.WaitForComplete();
                    }
                    else
                    {
                        callback.Dispose();
                    }
                }
                callbacks.Clear();
            }
        }
    }

    /// <summary>
    /// Custom thread type enum as the values in ENamedThreads will change depending on "#define STATS 0/1"
    /// </summary>
    public enum EAsyncThreadType
    {
        GameThread,
        RHIThread,
        RenderingThread,
        AnyThread,

        /// <summary>
        /// This is a special internal value which runs on GameThread but wont be wait for completion when
        /// hotreload unloads the assembly.
        /// </summary>
        GameThreadUnloadIgnore,
    }
}
