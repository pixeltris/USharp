using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FThreading
    {
        public delegate csbool Del_IsInGameThread();
        public delegate csbool Del_IsInSlateThread();
        public delegate csbool Del_IsInRenderingThread();
        public delegate csbool Del_IsInParallelRenderingThread();
        public delegate csbool Del_IsInActualRenderingThread();
        public delegate csbool Del_IsInAsyncLoadingThread();
        public delegate csbool Del_IsInRHIThread();
        public delegate csbool Del_IsRenderingThreadGameThread();

        public static Del_IsInGameThread IsInGameThread;
        public static Del_IsInSlateThread IsInSlateThread;
        public static Del_IsInRenderingThread IsInRenderingThread;
        public static Del_IsInParallelRenderingThread IsInParallelRenderingThread;
        public static Del_IsInActualRenderingThread IsInActualRenderingThread;
        public static Del_IsInAsyncLoadingThread IsInAsyncLoadingThread;
        public static Del_IsInRHIThread IsInRHIThread;
        public static Del_IsRenderingThreadGameThread IsRenderingThreadGameThread;
    }
}
