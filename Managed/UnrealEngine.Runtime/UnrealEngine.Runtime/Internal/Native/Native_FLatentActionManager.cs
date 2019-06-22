using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnrealEngine.Engine;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FLatentActionManager
    {
        public delegate void Del_ProcessLatentActions(IntPtr instance, IntPtr obj, float deltaTime);
        public delegate IntPtr Del_FindExistingActionUSharp(IntPtr instance, IntPtr actionObject, int uuid);
        public delegate IntPtr Del_FindExistingAction(IntPtr instance, IntPtr actionObject, int uuid);
        public delegate void Del_RemoveActionsForObject(IntPtr instance, ref FWeakObjectPtr obj);
        internal delegate IntPtr Del_AddNewAction(IntPtr instance, IntPtr actionObject, int uuid, IntPtr managedObject, ManagedLatentCallbackDel callback);
        public delegate void Del_BeginFrame(IntPtr instance);
        public delegate int Del_GetNumActionsForObject(IntPtr instance, ref FWeakObjectPtr obj);
        public delegate void Del_GetActiveUUIDs(IntPtr instance, IntPtr obj, IntPtr result);
        public delegate void Del_GetDescription(IntPtr instance, IntPtr obj, int uuid, ref FScriptArray result);
        public delegate int Del_GetNextUUID(IntPtr callbackTarget);

        public static Del_ProcessLatentActions ProcessLatentActions;
        public static Del_FindExistingActionUSharp FindExistingActionUSharp;
        public static Del_FindExistingAction FindExistingAction;
        public static Del_RemoveActionsForObject RemoveActionsForObject;
        internal static Del_AddNewAction AddNewAction;
        public static Del_BeginFrame BeginFrame;
        public static Del_GetNumActionsForObject GetNumActionsForObject;
        public static Del_GetActiveUUIDs GetActiveUUIDs;
        public static Del_GetDescription GetDescription;
        public static Del_GetNextUUID GetNextUUID;
    }
}
