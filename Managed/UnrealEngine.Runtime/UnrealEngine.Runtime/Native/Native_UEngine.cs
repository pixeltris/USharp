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
        public delegate void Del_GetWorldContexts(IntPtr instance, IntPtr result);
        public delegate IntPtr Del_GetWorldFromContextObject(IntPtr instance, IntPtr obj, EGetWorldErrorMode errorMode);

        public static Del_CopyPropertiesForUnrelatedObjects CopyPropertiesForUnrelatedObjects;
        public static Del_GetWorldContexts GetWorldContexts;
        public static Del_GetWorldFromContextObject GetWorldFromContextObject;
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
