using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FTimerManager
    {
        public delegate IntPtr Del_Tick(IntPtr instance, float deltaTime);
        public delegate void Del_SetTimer(IntPtr instance, ref FTimerHandle inOutHandle, ref FScriptDelegate dynDelegate, float rate, bool loop, float firstDelay);
        public delegate void Del_SetTimerForNextTick(IntPtr instance, ref FScriptDelegate dynDelegate);
        public delegate void Del_ClearTimer(IntPtr instance, ref FTimerHandle inOutHandle);
        public delegate void Del_ClearAllTimersForObject(IntPtr instance, IntPtr obj);
        public delegate void Del_PauseTimer(IntPtr instance, ref FTimerHandle handle);
        public delegate void Del_UnPauseTimer(IntPtr instance, ref FTimerHandle handle);
        public delegate float Del_GetTimerRate(IntPtr instance, ref FTimerHandle handle);
        public delegate csbool Del_IsTimerActive(IntPtr instance, ref FTimerHandle handle);
        public delegate csbool Del_IsTimerPaused(IntPtr instance, ref FTimerHandle handle);
        public delegate csbool Del_IsTimerPending(IntPtr instance, ref FTimerHandle handle);
        public delegate csbool Del_TimerExists(IntPtr instance, ref FTimerHandle handle);
        public delegate float Del_GetTimerElapsed(IntPtr instance, ref FTimerHandle handle);
        public delegate float Del_GetTimerRemaining(IntPtr instance, ref FTimerHandle handle);
        public delegate csbool Del_HasBeenTickedThisFrame(IntPtr instance);
        public delegate void Del_K2_FindDynamicTimerHandle(IntPtr instance, ref FScriptDelegate dynamicDelegate, ref FTimerHandle result);
        public delegate void Del_ListTimers(IntPtr instance);
        public delegate void Del_SetGameInstance(IntPtr instance, IntPtr gameInstance);

        public static Del_Tick Tick;
        public static Del_SetTimer SetTimer;
        public static Del_SetTimerForNextTick SetTimerForNextTick;
        public static Del_ClearTimer ClearTimer;
        public static Del_ClearAllTimersForObject ClearAllTimersForObject;
        public static Del_PauseTimer PauseTimer;
        public static Del_UnPauseTimer UnPauseTimer;
        public static Del_GetTimerRate GetTimerRate;
        public static Del_IsTimerActive IsTimerActive;
        public static Del_IsTimerPaused IsTimerPaused;
        public static Del_IsTimerPending IsTimerPending;
        public static Del_TimerExists TimerExists;
        public static Del_GetTimerElapsed GetTimerElapsed;
        public static Del_GetTimerRemaining GetTimerRemaining;
        public static Del_HasBeenTickedThisFrame HasBeenTickedThisFrame;
        public static Del_K2_FindDynamicTimerHandle K2_FindDynamicTimerHandle;
        public static Del_ListTimers ListTimers;
        public static Del_SetGameInstance SetGameInstance;
    }
}
