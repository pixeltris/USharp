using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 649 // Field is never assigned

namespace UnrealEngine.Runtime.Native
{
    static class Native_IConsoleManager
    {
        public delegate void FConsoleObjectVisitor(IntPtr a1, IntPtr a2);

        public delegate IntPtr Del_Get();
        public delegate IntPtr Del_RegisterConsoleVariableInt(IntPtr instance, ref FScriptArray name, int defaultValue, ref FScriptArray help, EConsoleVariableFlags flags);
        public delegate IntPtr Del_RegisterConsoleVariableFloat(IntPtr instance, ref FScriptArray name, float defaultValue, ref FScriptArray help, EConsoleVariableFlags flags);
        public delegate IntPtr Del_RegisterConsoleVariableString(IntPtr instance, ref FScriptArray name, ref FScriptArray defaultValue, ref FScriptArray help, EConsoleVariableFlags flags);
        public delegate void Del_CallAllConsoleVariableSinks(IntPtr instance);
        public delegate void Del_RegisterConsoleVariableSink_Handle(IntPtr instance, FConsoleCommandDelegate command, ref FDelegateHandle outHandle);
        public delegate void Del_UnregisterConsoleVariableSink_Handle(IntPtr instance, ref FDelegateHandle handle);
        public delegate IntPtr Del_RegisterConsoleCommandDefault(IntPtr instance, ref FScriptArray name, ref FScriptArray help, Delegate command, EConsoleVariableFlags flags);
        public delegate IntPtr Del_RegisterConsoleCommandWithArgs(IntPtr instance, ref FScriptArray name, ref FScriptArray help, Delegate command, EConsoleVariableFlags flags);
        public delegate IntPtr Del_RegisterConsoleCommandWithWorld(IntPtr instance, ref FScriptArray name, ref FScriptArray help, Delegate command, EConsoleVariableFlags flags);
        public delegate IntPtr Del_RegisterConsoleCommandWithWorldAndArgs(IntPtr instance, ref FScriptArray name, ref FScriptArray help, Delegate command, EConsoleVariableFlags flags);
        public delegate IntPtr Del_RegisterConsoleCommandWithOutputDevice(IntPtr instance, ref FScriptArray name, ref FScriptArray help, Delegate command, EConsoleVariableFlags flags);
        public delegate IntPtr Del_RegisterConsoleCommandExec(IntPtr instance, ref FScriptArray name, ref FScriptArray help, EConsoleVariableFlags flags);
        public delegate void Del_UnregisterConsoleObject(IntPtr instance, IntPtr consoleObject, csbool keepState);
        public delegate IntPtr Del_FindConsoleVariable(IntPtr instance, ref FScriptArray name);
        public delegate IntPtr Del_FindConsoleObject(IntPtr instance, ref FScriptArray name);
        public delegate void Del_ForEachConsoleObjectThatStartsWith(IntPtr instance, FConsoleObjectVisitor visitor, ref FScriptArray startsWith);
        public delegate void Del_ForEachConsoleObjectThatContains(IntPtr instance, FConsoleObjectVisitor visitor, ref FScriptArray contains);
        public delegate csbool Del_ProcessUserConsoleInput(IntPtr instance, ref FScriptArray input, IntPtr ar, IntPtr world);
        public delegate void Del_AddConsoleHistoryEntry(IntPtr instance, ref FScriptArray key, ref FScriptArray input);
        public delegate void Del_GetConsoleHistory(IntPtr instance, ref FScriptArray key, IntPtr outHistory);
        public delegate csbool Del_IsNameRegistered(IntPtr instance, ref FScriptArray name);

        public static Del_Get Get;
        public static Del_RegisterConsoleVariableInt RegisterConsoleVariableInt;
        public static Del_RegisterConsoleVariableFloat RegisterConsoleVariableFloat;
        public static Del_RegisterConsoleVariableString RegisterConsoleVariableString;
        public static Del_CallAllConsoleVariableSinks CallAllConsoleVariableSinks;
        public static Del_RegisterConsoleVariableSink_Handle RegisterConsoleVariableSink_Handle;
        public static Del_UnregisterConsoleVariableSink_Handle UnregisterConsoleVariableSink_Handle;
        public static Del_RegisterConsoleCommandDefault RegisterConsoleCommandDefault;
        public static Del_RegisterConsoleCommandWithArgs RegisterConsoleCommandWithArgs;
        public static Del_RegisterConsoleCommandWithWorld RegisterConsoleCommandWithWorld;
        public static Del_RegisterConsoleCommandWithWorldAndArgs RegisterConsoleCommandWithWorldAndArgs;
        public static Del_RegisterConsoleCommandWithOutputDevice RegisterConsoleCommandWithOutputDevice;
        public static Del_RegisterConsoleCommandExec RegisterConsoleCommandExec;
        public static Del_UnregisterConsoleObject UnregisterConsoleObject;
        public static Del_FindConsoleVariable FindConsoleVariable;
        public static Del_FindConsoleObject FindConsoleObject;
        public static Del_ForEachConsoleObjectThatStartsWith ForEachConsoleObjectThatStartsWith;
        public static Del_ForEachConsoleObjectThatContains ForEachConsoleObjectThatContains;
        public static Del_ProcessUserConsoleInput ProcessUserConsoleInput;
        public static Del_AddConsoleHistoryEntry AddConsoleHistoryEntry;
        public static Del_GetConsoleHistory GetConsoleHistory;
        public static Del_IsNameRegistered IsNameRegistered;
    }
}
