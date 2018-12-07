using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealEngine.Engine;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_FTickFunction
    {
        public delegate csbool Del_Get_HighPriority(IntPtr instance);
        public delegate void Del_Set_HighPriority(IntPtr instance, csbool value);
        public delegate csbool Del_Get_RunOnAnyThread(IntPtr instance);
        public delegate void Del_Set_RunOnAnyThread(IntPtr instance, csbool value);
        public delegate void Del_RegisterTickFunction(IntPtr instance, IntPtr level);
        public delegate void Del_UnRegisterTickFunction(IntPtr instance);
        public delegate csbool Del_IsTickFunctionRegistered(IntPtr instance);
        public delegate void Del_SetTickFunctionEnable(IntPtr instance, csbool enabled);
        public delegate csbool Del_IsCompletionHandleValid(IntPtr instance);
        public delegate ETickingGroup Del_GetActualTickGroup(IntPtr instance);
        public delegate ETickingGroup Del_GetActualEndTickGroup(IntPtr instance);
        public delegate void Del_AddPrerequisite(IntPtr instance, IntPtr targetObject, IntPtr targetTickFunction);
        public delegate void Del_RemovePrerequisite(IntPtr instance, IntPtr targetObject, IntPtr targetTickFunction);
        public delegate void Del_SetPriorityIncludingPrerequisites(IntPtr instance, csbool highPriority);
        public delegate IntPtr Del_GetPrerequisites(IntPtr instance);
        public delegate IntPtr Del_New(TickFunctionType type);
        public delegate void Del_Delete(IntPtr instance);

        public static Del_Get_HighPriority Get_bHighPriority;
        public static Del_Set_HighPriority Set_bHighPriority;
        public static Del_Get_RunOnAnyThread Get_bRunOnAnyThread;
        public static Del_Set_RunOnAnyThread Set_bRunOnAnyThread;
        public static Del_RegisterTickFunction RegisterTickFunction;
        public static Del_UnRegisterTickFunction UnRegisterTickFunction;
        public static Del_IsTickFunctionRegistered IsTickFunctionRegistered;
        public static Del_SetTickFunctionEnable SetTickFunctionEnable;
        public static Del_IsCompletionHandleValid IsCompletionHandleValid;
        public static Del_GetActualTickGroup GetActualTickGroup;
        public static Del_GetActualEndTickGroup GetActualEndTickGroup;
        public static Del_AddPrerequisite AddPrerequisite;
        public static Del_RemovePrerequisite RemovePrerequisite;
        public static Del_SetPriorityIncludingPrerequisites SetPriorityIncludingPrerequisites;
        public static Del_GetPrerequisites GetPrerequisites;
        public static Del_New New;
        public static Del_Delete Delete;
    }
}
