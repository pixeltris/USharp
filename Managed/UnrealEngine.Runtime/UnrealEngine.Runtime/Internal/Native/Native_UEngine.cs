using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Engine;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    public static class Native_UEngine
    {
        public delegate csbool Del_CopyPropertiesForUnrelatedObjects(IntPtr oldObject, IntPtr newObject, ref FCopyPropertiesForUnrelatedObjectsParams parameters);
        public delegate IntPtr Del_GetWorldFromContextObject(IntPtr obj, EGetWorldErrorMode errorMode);
        public delegate IntPtr Del_GetWorldContextFromWorld(IntPtr world);
        public delegate IntPtr Del_GetWorldContextFromGameViewport(IntPtr viewport);
        public delegate IntPtr Del_GetWorldContextFromPendingNetGame(IntPtr pendingNetGame);
        public delegate IntPtr Del_GetWorldContextFromPendingNetGameNetDriver(IntPtr pendingNetGame);
        public delegate IntPtr Del_GetWorldContextFromHandle(ref FName worldContextHandle);
        public delegate IntPtr Del_GetWorldContextFromPIEInstance(int pieInstance);
        public delegate IntPtr Del_GetWorldContextFromWorldChecked(IntPtr world);
        public delegate IntPtr Del_GetWorldContextFromGameViewportChecked(IntPtr viewport);
        public delegate IntPtr Del_GetWorldContextFromPendingNetGameCheckedChecked(IntPtr pendingNetGame);
        public delegate IntPtr Del_GetWorldContextFromPendingNetGameNetDriverChecked(IntPtr pendingNetGame);
        public delegate IntPtr Del_GetWorldContextFromHandleChecked(ref FName worldContextHandle);
        public delegate IntPtr Del_GetWorldContextFromPIEInstanceChecked(int pieInstance);
        public delegate void Del_GetWorldContexts(IntPtr result);

        public static Del_CopyPropertiesForUnrelatedObjects CopyPropertiesForUnrelatedObjects;
        public static Del_GetWorldFromContextObject GetWorldFromContextObject;
        public static Del_GetWorldContextFromWorld GetWorldContextFromWorld;
        public static Del_GetWorldContextFromGameViewport GetWorldContextFromGameViewport;
        public static Del_GetWorldContextFromPendingNetGame GetWorldContextFromPendingNetGame;
        public static Del_GetWorldContextFromPendingNetGameNetDriver GetWorldContextFromPendingNetGameNetDriver;
        public static Del_GetWorldContextFromHandle GetWorldContextFromHandle;
        public static Del_GetWorldContextFromPIEInstance GetWorldContextFromPIEInstance;
        public static Del_GetWorldContextFromWorldChecked GetWorldContextFromWorldChecked;
        public static Del_GetWorldContextFromGameViewportChecked GetWorldContextFromGameViewportChecked;
        public static Del_GetWorldContextFromPendingNetGameCheckedChecked GetWorldContextFromPendingNetGameCheckedChecked;
        public static Del_GetWorldContextFromPendingNetGameNetDriverChecked GetWorldContextFromPendingNetGameNetDriverChecked;
        public static Del_GetWorldContextFromHandleChecked GetWorldContextFromHandleChecked;
        public static Del_GetWorldContextFromPIEInstanceChecked GetWorldContextFromPIEInstanceChecked;
        public static Del_GetWorldContexts GetWorldContexts;
    }

    // NOTE: Wrapper due to bool (see NativeFunctions "BoolInteropNotes")
    // TODO: Move this struct somewhere else?

    /// <summary>
    /// Makes a strong effort to copy everything possible from and old object to a new object of a different class, 
    /// used for blueprint to update things after a recompile.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FCopyPropertiesForUnrelatedObjectsParams
    {
        public csbool AggressiveDefaultSubobjectReplacement;
        public csbool DoDelta;
        public csbool ReplaceObjectClassReferences;
        public csbool CopyDeprecatedProperties;
        public csbool PreserveRootComponent;

        // Skips copying properties with BlueprintCompilerGeneratedDefaults metadata 
        public csbool SkipCompilerGeneratedDefaults;
        public csbool NotifyObjectReplacement;
        public csbool ClearReferences;

        public static FCopyPropertiesForUnrelatedObjectsParams Default
        {
            get
            {
                return new FCopyPropertiesForUnrelatedObjectsParams()
                {
                    AggressiveDefaultSubobjectReplacement = false,
                    DoDelta = false,
                    ReplaceObjectClassReferences = true,
                    CopyDeprecatedProperties = false,
                    PreserveRootComponent = true,
                    SkipCompilerGeneratedDefaults = false,
                    NotifyObjectReplacement = true,
                    ClearReferences = true
                };
            }
        }
    }
}
