using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UWorld
    {
        public delegate IntPtr Del_Get_GWorld();
        public delegate IntPtr Del_GetLevels(IntPtr instance);        
        public delegate IntPtr Del_GetGameInstance(IntPtr instance);
        public delegate IntPtr Del_GetTimerManager(IntPtr instance);
        public delegate IntPtr Del_SpawnActor(IntPtr instance, IntPtr Class, ref FVector Location, ref FRotator Rotation, ref FActorSpawnParameters parameters);

        public static Del_Get_GWorld Get_GWorld;
        public static Del_GetLevels GetLevels;
        public static Del_GetGameInstance GetGameInstance;
        public static Del_GetTimerManager GetTimerManager;
        public static Del_SpawnActor SpawnActor;
    }

    /// <summary>
    /// FActorSpawnParameters
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FActorSpawnParameters
    {
        public FName Name;
        public IntPtr Template;
        public IntPtr Owner;
        public IntPtr Instigator;
        public IntPtr OverrideLevel;
        public ESpawnActorCollisionHandlingMethod SpawnCollisionHandlingOverride;
    }
}
